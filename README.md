# Bitcoin Price App

Jednoduchá WPF aplikace pro Windows, která zobrazuje aktuální cenu Bitcoinu v USD a v CZK. Často se aktualizuje, běží v oznamovací oblasti (tray) a umožňuje rychlý přehled o vývoji ceny. Zobrazuje také datum a čas poslední úspěšné aktualizace.

![screenshot](https://raw.githubusercontent.com/foldas/btc-price-app/master/.github/images/app.jpg)

## Vlastnosti

- **Zobrazení BTC v USD a CZK**  
  Základní přehled ceny Bitcoinu na jednom místě.  
- **Čas aktualizace**  
  Cena BTC se obnovuje každých 30 vteřin.  
- **Poslední aktualizace**  
  Uvádí přesné datum a čas naposledy načtených dat.  
- **Tray ikona**  
  Aplikace běží na pozadí a lze ji snadno vyvolat z oznamovací oblasti.  
- **Žádná instalace**  
  Lze sestavit jako samostatný EXE soubor (self-contained). Stačí spustit.

## Požadavky

- Windows 10 nebo novější (testováno na Windows 11).
- .NET Runtime, pokud aplikaci nepublikujete jako self-contained.

## Jak aplikaci používat

1. **Spusťte EXE** (`BtcPriceApp.exe`).  
2. **Hlavní okno** zobrazí dvě hodnoty: cenu Bitcoinu v USD a v CZK.  
3. **Tray ikona** se objeví vpravo dole, kde můžete aplikaci kdykoli otevřít či ukončit.

## Stažení aplikace

[Stáhnout BtcPriceApp.exe (v1.0.0)](https://github.com/foldas/btc-price-app/releases/download/v1.0.0/BtcPriceApp.exe)

### Sestavení a publikace

1. Otevřete projekt v IDE (Visual Studio, Visual Studio Code) nebo v terminálu.
2. Spusťte `dotnet publish -c Release` pro vytvoření publikovatelné verze.  
3. Pokud chcete samostatný EXE bez nutnosti .NET Runtime, použijte parametry:
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
   ```
4. Spustitelný soubor najdete ve složce bin/Release/netX.0-windows/win-x64/publish
