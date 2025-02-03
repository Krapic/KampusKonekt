# BazePodatakaProjekt

Ovo je web aplikacija razvijena u sklopu kolegija **Baze podataka**. Aplikacija je namijenjena studentima i omogućuje objavljivanje i pregledavanje oglasa, ostavljanje recenzija, te upravljanje korisničkim profilima. Projekt je razvijen koristeći **ASP.NET Core 9 MVC** i **Microsoft SQL Server** kao bazu podataka.

---

## Sadržaj

- [Opis projekta](#opis-projekta)
- [Tehnologije](#tehnologije)
- [Funkcionalnosti](#funkcionalnosti)
- [Postavljanje projekta](#postavljanje-projekta)
- [Struktura baze podataka](#struktura-baze-podataka)
- [Autor](#autor)

---

## Opis projekta

Aplikacija je zamišljena kao platforma slična eBay-u ili Njuškalu, ali namijenjena prvenstveno studentima. Cilj je olakšati razmjenu predmeta, usluga i informacija unutar studentske zajednice.

---

## Tehnologije

Projekt koristi sljedeće tehnologije i alate:

- **ASP.NET Core 9 MVC** za razvoj web aplikacije.
- **Entity Framework Core** za upravljanje bazom podataka.
- **Microsoft SQL Server** za pohranu podataka.
- **Bootstrap** za responzivan dizajn sučelja.
- **Git i GitHub** za verzioniranje koda.

---

## Funkcionalnosti

### Korisnici
- Registracija i prijava korisnika.
- Upravljanje korisničkim profilom (ime, prezime, fakultet, opis, profilna slika).
- Pregled objavljenih oglasa.
- Pregled omiljenih oglasa.
- Pregled broja pratitelja i osoba koje korisnik prati.

### Oglasi
- Kreiranje, uređivanje i brisanje oglasa.
- Pregled oglasa po kategorijama, cijeni ili datumu.
- Dodavanje oglasa u favorite.

### Kategorije
- Upravljanje kategorijama oglasa (za administratore).
- Pregled oglasa unutar odabrane kategorije.

### Recenzije
- Ostavljanje ocjena i komentara na oglase.
- Pregled prosječne ocjene oglasa ili korisnika.

### Dodatne funkcionalnosti
- Pretraga i filtriranje oglasa prema različitim kriterijima.
- Administracija korisnika i oglasa (blokiranje korisnika, uklanjanje neprikladnih oglasa).

---

## Postavljanje projekta

Slijedite ove korake za postavljanje projekta lokalno:

1. **Kloniraj repozitorij:**
   ```bash
   git clone https://github.com/tvoje-korisnicko-ime/BazePodatakaProjekt.git
   cd BazePodatakaProjekt
2. **Ažuriraj bazu podataka:**
3. ```bash
   Update-Database
