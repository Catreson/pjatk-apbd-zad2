# pjatk-apbd-zad2
Ćwiczenia 2 APBD
Projekt realizujący wypożyczalnię sprzętu dla uczelni.
Umożliwia dodanie i usunięcie sprzętu, dodanie i usunięcie użytkownika, wypożyczenie i zwrócenie (z możliwą karą), wyświetlenie wszystkich urządzeń/wypożyczeń/użytkowników, zablokowanie urządzenia, wygenerowanie raportu.  
  
"Użytkownik" to osoba korzystająca z wypożyczalni, nie z programu. Użytkownik programu z założenia jest osobą obsługującą w całości wypożyczalnię.  

Projekt zapisuje plik data.json w katalogu gdzie znajduje się plik wykonywalny.  

Do uruchomienia projektu należy w katalogu `Cwiczenia2` uruchomić polecenie `dotnet run`  

Kohezja - RentalService zarządza logiką wypożyczenia i tylko nią.

Coupling - Menu związane jest bezpośrednio ze wszystkimi klasami.

Odpowiedzialność klasy - DataService zarządza przechowywaniem danych, RentalService zarządza operacjami na nich.

Podzielone są w ten sposób, ponieważ wszędzie to jest sugerowane. Klasy Models jako warstwa modeli danych, klasy Services jako warstwa serwisów do pracy z danymi i zarządzania nimi, Menu jako interfejs użytkownika wiążący input z operacjami na i tworzeniem danych, a także formatujący wyświetlane dane. 
