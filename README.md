# FairPlan — stamdata til vagtplanlægning

Webapplikation til at vedligeholde de stamdata, en vagtplan bygger på: hvilke
perioder der planlægges for, hvilke afsnit og personalegrupper hospitalet har,
hvem der er oprettet i systemet, og hvem der er ansat hvor og hvornår.

Selve vagtplanlægningen er ikke en del af løsningen.

## Kør projektet

Kræver .NET SDK 8 eller nyere og Node 20 eller nyere.

**Backend**

```
cd backend
dotnet run --project src/WebAPI
```

Kører på http://localhost:5166. Swagger ligger på http://localhost:5166/swagger.

Databasen er SQLite og oprettes automatisk ved første opstart som
`backend/src/WebAPI/fairplan.db`. Der skal ikke installeres eller konfigureres
noget.

**Frontend**

```
cd frontend
npm install
npm run dev
```

Kører på http://localhost:5173. Backenden skal køre samtidig.

## Opbygning

Backenden følger templatets lagdeling (Onion Architecture):

| Lag | Indhold |
|---|---|
| `Core` (Domain) | Entiteter, DTO'er og interfaces |
| `Application` | Services med forretningslogikken |
| `Infrastructure` | EF Core, DbContext og repositories |
| `WebAPI` | Controllers og opsætning |

Afhængighederne peger indad: Application og Infrastructure kender Domain, men
ikke hinanden. Forretningsreglerne i Application kender hverken database eller
HTTP.

Frontenden er React og TypeScript med React Router v7 i SPA-tilstand. Alle
tabeller bruger den samme komponent, `DataTable`, som håndterer søgning,
sortering og filtrering.

## Forretningsregler

Alle regler håndhæves i backenden og formidles til brugeren som tydelige
fejlbeskeder.

**Perioder**

- Startdato skal være før eller samme dag som slutdato
- Perioder må ikke overlappe hinanden. De må gerne ligge umiddelbart efter

**Afsnit og personalegrupper**

- Kan ikke slettes, hvis der er tilknyttede ansættelser
- Afsnit skal have en gyldig HEX-farvekode

**Adgange**

- E-mail skal være unik og en gyldig e-mailadresse
- En bruger skal have mindst én rolle
- Kan ikke slettes, hvis brugeren har ansættelser

**Ansættelser**

- Slutdato skal være efter startdato
- Timer pr. uge skal være et helt tal mellem 0 og 37
- Personen skal have rollen Personale
- En persons ansættelser må ikke overlappe hinanden

**Personale**

- En person indgår, hvis ansættelsen overlapper den valgte periode

## Tekniske valg

**SQLite frem for SQL Server.** Opgaven kræver permanent datalagring, og en
SQLite-fil opfylder det. Valget betyder, at løsningen kan klones og køres uden
opsætning. EF Core gør det til en linjes arbejde at skifte provider, hvis den
skulle i produktion.

**Eget domæne frem for templatets.** Templatets eksempel-entiteter beskriver en
anden model end opgaven — blandt andet sidder afsnit og lægetype på brugeren og
ikke på ansættelsen. Da en person kan skifte afsnit over tid, er ansættelsen det
rigtige sted. Templatets lagdeling og opsætning er beholdt.

**Reglerne i backenden, ikke i brugerfladen.** Frontenden hjælper brugeren med
at undgå fejl — kun personale kan vælges i ansættelsesformularen, og farver
vælges med en farvevælger — men reglerne står i backenden og gælder uanset,
hvem der kalder API'et.

**Én tabelkomponent.** Søgning, sortering og filtrering er bygget én gang og
genbruges på alle seks tabeller. Personale-siden bruger samme komponent uden
handlingsknapper og er dermed skrivebeskyttet.

**Dansk til brugeren, engelsk i koden.** Klassenavne og kode er engelsk efter
konvention. Fejlbeskeder er danske, fordi de vises til dansk hospitalspersonale.

## Afgrænsninger og spørgsmål

**Overlappende ansættelser på tværs af afsnit.** Opgaven siger, at en persons
ansættelser ikke må overlappe. Det er implementeret som skrevet, uanset afsnit.
I praksis kunne en læge være ansat deltid to steder samtidig, så det er et
spørgsmål, jeg ville stille forretningen.

**Perioder kan slettes frit.** Opgaven nævner ingen sletteregel for perioder, og
ansættelser peger ikke på en periode — sammenhængen beregnes ud fra datoer. En
sletning efterlader derfor ingen forældreløse data.

**Ingen brugerstyring.** Rollerne Admin og Personale er felter på brugeren og
bruges som valideringsregel ved ansættelser. Opgaven beskriver ikke login eller
rollebaseret adgang, så det er ikke bygget.
