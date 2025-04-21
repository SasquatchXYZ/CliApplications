using System.CommandLine;
using BookmarkrV9.Services.BookmarkService;

namespace BookmarkrV9.Commands.Link.Add;

public class LinkAddCommand : Command
{

    #region Properties

    private readonly IBookmarkService _bookmarkService;

    #endregion

    #region Constructor

    public LinkAddCommand(
        IBookmarkService bookmarkService,
        string name,
        string? description = null) : base(name, description)
    {
        _bookmarkService = bookmarkService;

        AddOption(nameOption);
        AddOption(urlOption);
        AddOption(categoryOption);

        this.SetHandler(OnHandleAddLinkCommand, nameOption, urlOption, categoryOption);

        ConfigureOptions();
    }

    private void ConfigureOptions()
    {
        urlOption.AddValidator(result =>
        {
            foreach (var token in result.Tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Value))
                {
                    result.ErrorMessage = "Url cannot be empty";
                    break;
                }

                if (Uri.TryCreate(token.Value, UriKind.Absolute, out _))
                    continue;

                result.ErrorMessage = $"Invalid Url: {token.Value}";
                break;
            }
        });

        categoryOption.SetDefaultValue("Read Later");
        categoryOption.FromAmong("Read Later", "Tech Books", "Cooking", "Social Media", "Cars");
        categoryOption.AddCompletions("Read Later", "Tech Books", "Cooking", "Social Media", "Cars");
    }

    #endregion

    #region Options

    private Option<string[]> nameOption = new(
        ["--name", "-n"],
        "The name of the bookmark"
    )
    {
        IsRequired = true,
        Arity = ArgumentArity.OneOrMore,
        AllowMultipleArgumentsPerToken = true,
    };

    private Option<string[]> urlOption = new(
        ["--url", "-u"],
        "The Url of the bookmark"
    )
    {
        IsRequired = true,
        Arity = ArgumentArity.OneOrMore,
        AllowMultipleArgumentsPerToken = true,
    };

    private Option<string[]> categoryOption = new(
        ["--category", "-c"],
        "The category to which the bookmark is associated."
    )
    {
        Arity = ArgumentArity.OneOrMore,
        AllowMultipleArgumentsPerToken = true,
    };

    #endregion

    #region Handler method

    private void OnHandleAddLinkCommand(string[] names, string[] urls, string[] categories)
    {
        _bookmarkService.AddLinks(names, urls, categories);
        _bookmarkService.ListAll();
    }

    #endregion

}
