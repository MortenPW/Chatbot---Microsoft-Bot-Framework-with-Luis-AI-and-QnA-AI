using System;
using System.Collections.Generic;

namespace PaaminnerPaal
{
    [Serializable]
    public sealed class ConfigurationOptions
    {
        private static readonly Lazy<ConfigurationOptions> Lazy =
            new Lazy<ConfigurationOptions>(() => new ConfigurationOptions());

        public static ConfigurationOptions Instance => Lazy.Value;

        // Base uri
        //public const string RootUri = "http://localhost:3984/";

        // Azure Content Delivery Network Blob
        // (Simple properties- not variables)
        public string RootUri => "https://paal2b79c.blob.core.windows.net/paal/";
        public string ImgDir => "images/";

        // Kolonial.no token
        // https://github.com/kolonialno/api-docs
        public string KolonialKey => ""; // Token
        public string KolonialId => "MortenWilsgaard_Test"; // UserAgent
        public string KolonialHost => "https://kolonial.no/api/v1/";

        // LUIS token
        // https://eu.luis.ai
        public string LuisKey => "";
        public string LuisId => "";
        public string LuisHost => "https://northeurope.api.cognitive.microsoft.com/luis/v2.0/apps/";

        // QnA token
        public string QnaAuthKey => "";
        public string QnaKbId => "";
        public string EndpointHostName => "https://paal2qna.azurewebsites.net/qnamaker";

        public string ApiKeyTextAnalytics => "";

        public readonly Dictionary<string, string[]> IntentResponses;

        // Text analytics uses Azure

        // TODO: Should be moved into resource class or file
        // Resource file (avoid hard coding strings):
        // https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/demo-ContosoFlowers#localization

        // List of allergies
        // https://www.naaf.no/subsites/matallergi/kostrad-ved-allergi/14-allergifremkallende-ingredienser/

        public string[] Allergies => new[]
        {
            "Glutenholdig korn", "Skalldyr", "Egg", "Fisk", "Peanøtter", "Soya", "Melk (herunder laktose)",
            "Nøtter", "Selleri", "Sennep", "Sesamfrø", "Svoveldioksid og sulfitt", "Lupin", "Bløtdyr"
        };

        public string[] Greeting => new[]
        {
            "Hei!", "Hallo!", "Hei på deg!", "Hei, alt bra?", "Hallo, har du det bra?", "Hei, hvordan står det til?",
            "Hei, hatt en fin dag?", "Hallo, hvordan går det med deg?"
        };

        public string[] NotUnderstood => new[]
        {
            "Beklager, jeg forstår ikke hva du mener. Kan du omformulere det?", "Jeg forstår ikke, kan du omformulere?",
            "Jeg skjønner ikke helt, kan du endre formuleringen?",
            "Den forsto jeg dessverre ikke, kan du formulere deg annerledes?",
            "Jeg forstår dessverre ikke hva du mener, kan du omformulere deg?"
        };

        public string[] More => new[]
        {
            "Vil du ha flere? :)", "Noen flere?", "Vil du ha flere?", "Vil du ha noen til? :)",
            "Ønsker du flere?", "Skal det være noen flere?", "Vil du ha noen andre? :)", "Noen til? :)"
        };

        private ConfigurationOptions()
        {
            IntentResponses = new Dictionary<string, string[]>
            {
                { "WhatsYourName", WhatsYourName },
                { "YoureWelcome", YoureWelcome },
                { "Bye", Bye },
                { "IkkeSant", IkkeSant },
                { "Fysj", Fysj },
                { "HarDetFint", HarDetFint },
                { "Deny", Deny },
                { "Confirm", Confirm },
                { "Advice", Advice },
                { "Joke", Joke }
            };
        }

        private string[] WhatsYourName => new[]
        {
            "Mitt navn er Pål v. 0.1.", "Jeg er Pål v. 0.1.", "Jeg er Påminner-Pål!"
        };

        private string[] YoureWelcome => new[]
        {
            "Alt for deg! :)", "For deg? Skulle bare mangle!", "Værsågod!", "Ingen problem! :)"
        };

        private string[] Bye => new[]
        {
            "Vi snakkes!", "Hadet! :)", "Bare ta kontakt om du lurer på noe! Hadet! :)", "Adjø! =)", "Farvel! :)"
        };

        private string[] IkkeSant => new[]
        {
            "Ikke sant!", "Ja!", "Råkult!", "Kult!"
        };

        private string[] Fysj => new[]
        {
            "Fysj!", "Det var ikke pent!", "Uffda.", "Sukk."
        };

        private string[] HarDetFint => new[]
        {
            "Jeg har det bra, kan jeg hjelpe deg?", "Ja, jeg har det fint. Hva med deg?", "Jeg har det veldig bra!",
            "Kjempefint! Kan jeg hjelpe deg med noe?", "Har det supert! Kan jeg hjelpe deg med noe?"
        };

        private string[] Deny => new[]
        {
            "Nei?", "Nei...?"
        };

        private string[] Confirm => new[]
        {
            "Ja?", "Ja...?"
        };

        private string[] Advice => new[]
        {
            "Spør de gamle til råds og de unge etter hjelp.",
            "Du bør ha krysset elven før du forteller krokodillen at den har dårlig ånde.",
            "Du bør fylle livet ditt med erfaringer, ikke unnskyldninger.",
            "Det er ikke lett å diskutere med noen som har rett.",
            "Mange avviser en god idé kun fordi den ikke er deres egen.",
            "Det er to ting å strebe etter i livet: først å få det en ønsker seg, og så å nyte det. Bare de klokeste oppnår det siste.",
            "Du får aldri belønning for det du hadde tenkt å gjøre.",
            "Det meste av det man bekymrer seg for inntreffer aldri.",
            "Det kommer mer an på hvem du er enn hvor du drar når du vil møte interessante folk.",
            "Formålet med kritikk er hjelp, ikke ydmykelse.",
            "Du får aldri utrettet noe dersom du skal vente til alt ligger til rette.",
            "Hvis du aldri tar feil er det kanskje fordi du ikke tar nok sjanser.",
            "Mange mennesker interesserer seg altfor mye for hva de har og alt for lite for hva de er.",
            "Kjærlighet er ikke å kreve av andre, det er å gi av seg selv.",
            "Den som spør, kan virke dum i fem minutter, men den som lar være, forblir dum.",
            "Kunsten å være lykkelig består i gjøre sine gleder enkle.",
            "Intet tre vokser inn i himmelen.",
            "Se mot framtiden, fortiden er borte for godt.",
            "Smuler er også brød.",
            "Bøker gjør folk både kloke og gale.",
            "Ingen er så stor at han ikke må tøye seg, ingen så liten at han ikke må bøye seg.",
            "Får man ikke den man elsker, får man elske den man får.",
            "Om hundre år er allting glemt.",
            "Den som frir til penger, spør ikke om alderen.",
            "Alltid husk at du er helt unik. Akkurat som alle andre.",
            "Inne i meg bor det en tynn kvinne som roper for å komme ut.Men jeg klarer vanligvis å få henne til å klappe igjen ved hjelp av kjeks.",
            "For all del gift deg.Hvis du finner ei god kone, kommer du til å bli lykkelig.Finner du ei dårlig kone, kommer du til å bli filosof.",
            "Før du kritiserer noen, prøv å gå en mil i hans sko.På den måte er du forsikret om at dersom han blir sint, vil han være en mil unna – barfot.",
            "Ingenting føles verre enn det øyeblikket i en diskusjon når du oppdager at du har feil.",
            "Jeg skal la meg imponere av teknologi den dagen de gjør det mulig å laste ned mat.",
            "Hemmeligheten bak et vellykket forhold er å huske å slette nettleserhistorikken."
        };

        private string[] Joke => new[]
        {
            "Katta til Fritz",
            "Hvorfor har svenskene et kålhode i hanskerommet? \n- Fordi de bruker det som legitimasjon.",
            "Forskrekket mor til sin sønn: – Men lille Jan da, hvordan er det du ser ut? Klærne dine er jo fulle av hull! – Du skjønner, vi har lekt butikk. Kjell var kjøpmann og jeg var sveitserost.",
            "– Pappa, jeg synes så synd på den damen som står der borte og skriker. Har du 20 kroner? – Ja, så klart, her er 20 kroningen. Hva skriker hun da? – Jordbæris, 20 kroner.",
            "– Mamma, mamma, vet du hvor mye tannkrem det er i en tannkremtube? – Nei – Det vet jeg, det er to ganger frem og tilbake over sofaen.",
            "Naboen: – Er det din ball som er kommet inn i hagen min, Stein? Stein: – Er det knust noen ruter? Naboen: – Nei. Stein: – Da er det min ball.",
            "– Kan du være så snill og gi meg noen mynter til myntsamlingen min, pappa? – Det er en interessant hobby, Lars. Hva slags mynter samler du på da? – Alle slags som mannen i pølseboden vil ta i mot.",
            "Lille Ole er ute og aker. Så krasjer han med et tre og utbryter: – Tre mot en er feigt.",
            "Læreren: – Mette du må lære deg å skrive tydeligere. Mette: Det er ikke nødvendig, jeg skal bli lege.",
            "Far: – Ole, hvor fikk du den DVD-spilleren fra? Ole: – Jeg solgte TV-en vår.",
            "– Vil du ha en termos? – Gjerne – Au, hvorfor tråkka du på tærne mine? – Du sa jo tær-mos."
        };
    }
}