# Homentare k reseni:

## Deskriptovni tridy 
Pro kazdou POCO tridu jsem vytvoril statickou deskriptnivni tridu, ktera obsahuje Dictionary vsech fieldu dane tridy. Klic je nazev fieldu a value je Action, ktera do writeru zapise danou hodnotu fieldu v XML formatu. Pokud field obsahuje tridu, vytvori si RootDeskriptor danne tridy a zavola na ni Serialize().  
Diky tomu, ze nevraci hodnotu fieldu, ale rovnou field zapisuje do writeru, nezalezi na typu fieldu.


## Get...Descriptor()
Pro kazdou specializaci RootDescriptoru jsem napsal funkci, kde se opakuje kod pro jednotlive tridy - zde je moznost pro vylepseni, tato funkce by mohla byt genericka. Funkce vytvori specializaci noveho deskriptoru a ulozi do pole *action* vypsani vsech fieldu dane tridy z dane deskriptivni tridy.

## RootDescriptor
Jeho Serialize() na instanci vypise do TextWriteru oteviraci a zaviraci element a mezi ne zavola ulozene funkce v poli *action*

## XMl
XML je staticka pomocna trida, ktera konvertuje hodnoty a nazvy elementu do XML formatu.