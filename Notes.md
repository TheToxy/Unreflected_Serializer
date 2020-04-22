Ondrej Kraus
# Komentare k reseni:

## Deskriptovni tridy 
Vytvoril jsem statickou deskriptnivni tridu, ktera obsahuje Dictionary vsech fieldu pro kazdou POCO tridu. Klic je nazev fieldu a value je Action, ktera do writeru zapise danou hodnotu fieldu v XML formatu. Pokud field obsahuje tridu (neni to jen string nebo int), vytvori si RootDeskriptor danne tridy a zavola na ni Serialize().  
Diky tomu, ze nevraci hodnotu fieldu, ale rovnou field zapisuje do writeru, nezalezi na typu fieldu a bude tedy fungovat i kdyz pridam typ char nebo bool.


## Get...Descriptor()
Funkce vytvori specializaci noveho deskriptoru a ulozi do pole *action* vypsani vsech fieldu dane tridy z dane deskriptivni tridy.  
Po uvaze jsem vytvoril **dve branche** - jedna zachovava puvodni kod main(), druha pouziva genericky GetGenericDescriptor<T\>()  

Vyhoda genericke metody je ve flexibilite, sice musim predavat jako argument Dictionary popisujici danou tridu, ale tim padem je to konfigurovatelne (jake fieldy se budou vypisovat a jak). Nepekne je, ze reseni je svazane s XML formatem a lepsi by bylo predavat i serializacni funkci jako parametr (musel by se zmenit i Serialize() v RootDescriptoru).


## RootDescriptor
Jeho Serialize() na instanci vypise do TextWriteru oteviraci a zaviraci element a mezi ne zavola ulozene funkce v poli *action*

## XMl
XML je staticka pomocna trida, ktera konvertuje hodnoty a nazvy elementu do XML formatu.