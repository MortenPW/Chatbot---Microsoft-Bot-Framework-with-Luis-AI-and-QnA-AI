using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;

// Language:
// https://github.com/Microsoft/BotBuilder/tree/develop/CSharp/Library/Microsoft.Bot.Builder/MultilingualResources
// https://github.com/Microsoft/BotBuilder-Location/blob/master/CSharp/BotBuilderLocation/LocationResourceManager.cs
// https://docs.botframework.com/en-us/csharp/builder/sdkreference/dc/dfd/_attributes_8cs_source.html

// NB. bug causing problems: https://github.com/Microsoft/BotBuilder/issues/2589
// TODO: Issue persists even after upgrading

public enum AllergyOptions
{
    [Terms("alt bortsett fra", "alt", "alle")]
    GlutenholdigKorn,
    Skalldyr,
    Egg,
    Fisk,
    Peanøtter,
    Soya,
    MelkLaktose,
    Nøtter,
    Selleri,
    Sennep,
    Sesamfrø,
    SvoveldioksidOgSulfitt,
    Lupin,
    Bløtdyr
}

public enum LengthOptions
{
    SixInch,
    FootLong
}

[Serializable]
public class Allergy
{
    [Template(TemplateUsage.NotUnderstood, "Sikker på at du skrev riktig? Jeg forstår ikke \"{0}\".",
        "Prøv igjen, jeg forstår ikke \"{0}\".")]
    [Template(TemplateUsage.Clarify, "Sikker")]
    [Template(TemplateUsage.Confirmation, "Sikker")]
    [Template(TemplateUsage.CurrentChoice, "\n\nValgt: {*}")]
    [Template(TemplateUsage.Feedback, "Sikker")]
    [Prompt("Hva slags {&} vil du legge til? {||}")]
    public List<AllergyOptions> Allergener;

    public LengthOptions? Length;

    public static IForm<Allergy> BuildForm()
    {
        var form = new FormBuilder<Allergy>()
            .Message("Her kan du legge inn ulike allergener!")
            //.Confirm("Er dette riktige valg?", state => false)
            //.Field(nameof(Allergener)) // If this is added, selection confirmation is skipped
            ;

        form.Configuration.Yes = new[] {"ja", "ye", "ya", "yes", "riktig"};
        form.Configuration.No = new[] {"nei", "neh", "no", "feil"};

        form.Configuration.Confirmation = "STEMMER DETTE A?";
        form.Configuration.Navigation = "STEMMER DETTE A?";
        form.Configuration.CurrentChoice = new[] { "STEMMER DETTE A?" };
        form.Configuration.NoPreference = new[] { "STEMMER DETTE A?" };

        //form.Configuration.DefaultPrompt.ChoiceLastSeparator = "STEMMER DETTE A?";

        // Commands: https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.builder.formflow.formcommand?view=botbuilder-3.12.2.4
        form.Configuration.Commands[FormCommand.Quit].Terms = new[] {"avbryt", "stopp", "avslutt", "gi faen"};

        form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Auto;
        //form.Configuration.DefaultPrompt.Feedback = FeedbackOptions.Always;
        form.Configuration.DefaultPrompt.IsLocalizable = true;

        return form.Build();
    }
}