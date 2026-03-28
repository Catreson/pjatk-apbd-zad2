using Cwiczenia2.CLI;
using Cwiczenia2.Services;

var dataService = new DataService();
var store = DataService.Load();
var rentalService = new RentalService(store, dataService);
var menu = new Menu(rentalService);

menu.Run();