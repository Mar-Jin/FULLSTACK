namespace RevaliNow.Services;

// Fouten die een service kan geven. De controller vertaalt ze naar de juiste HTTP-statuscode,
// zodat de bedrijfslogica niets van HTTP hoeft te weten.

// Het gevraagde record bestaat niet (of hoort niet bij deze patiënt) -> 404
public class NietGevondenException(string message) : Exception(message);

// De actie mag volgens de bedrijfsregels niet -> 400
public class OngeldigeActieException(string message) : Exception(message);
