using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {

        internal static List<USState> GetStates()
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = zipCode;
                newAddress.USStateId = stateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static Adoption[] GetPendingAdoptions()
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            var pendingAdoptions = db.Adoptions.Where(m => m.ApprovalStatus == "pending");

            Adoption[] AdoptablesArray = pendingAdoptions.ToArray();

            return AdoptablesArray;
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = clientAddress.Zipcode;
                newAddress.USStateId = clientAddress.USStateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }

        internal static Room GetRoom(int animalId)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            Room selectedRoom = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();

            return selectedRoom;
        }

        internal static void UpdateAdoption(bool v, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RunEmployeeQueries(Employee employee, string v)
        {
            throw new NotImplementedException();
        }

        internal static Animal[] SearchForAnimalByMultipleTraits()
        {
            throw new NotImplementedException();
        }

        internal static Animal GetAnimalByID(int iD)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            Animal selectedAnimal = db.Animals.Where(a => a.AnimalId == iD).FirstOrDefault();

            return selectedAnimal;
        }

        internal static void Adopt(object animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if(employeeFromDb == null)
            {
                throw new NullReferenceException();            
            }
            else
            {
                return employeeFromDb;
            }            
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static AnimalShot[] GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static void UpdateShot(string shotType, Animal animal)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            AnimalShot newShot = new AnimalShot();
            Shot selectedShot = db.Shots.Where(m => m.Name.ToLower() == shotType.ToLower()).FirstOrDefault();

            newShot.AnimalId = animal.AnimalId;
            newShot.ShotId = selectedShot.ShotId;

            db.AnimalShots.InsertOnSubmit(newShot);

            db.SubmitChanges();
        }

        internal static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static void AddAnimal(Animal animal)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static int GetDietPlanId()
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            var dietPlans = db.DietPlans;

            string name = null;
            string type = null;

            int amount = 0;

            Console.WriteLine("Current animal diets listed are: ");
            foreach (var diet in dietPlans)
            {
                Console.WriteLine("Name: "+diet.Name+", food type: "+diet.FoodType+", Amount of food in cups: "+diet.FoodAmountInCups);
            }

            Console.WriteLine("Type out the number associated with your animal's diet.");
            Console.WriteLine("If your animal does not have a diet plan, you may add one by typing in 'add'.");

            string input = Console.ReadLine();
            bool validInput = false;

            if (input.ToLower() == "add")
            {
                DietPlan newDiet = new DietPlan();

                while (!validInput)
                {
                    Console.WriteLine("You have selected to add a diet plan. What is its name?");
                    name = Console.ReadLine();
                    Console.WriteLine("What food type is it?");
                    type = Console.ReadLine();
                    Console.WriteLine("How many cups are in one serving?");
                    amount = int.Parse(Console.ReadLine());

                    Console.WriteLine("I see, so the name of the category is " + name + ", ");
                    Console.WriteLine("the type of food is " + type + ", ");
                    Console.WriteLine("and the amount of cups in one serving is "+amount.ToString()+".");

                    Console.WriteLine("Is all of the above information correct y/n?");
                    string confirmInput = Console.ReadLine();
                    switch (confirmInput.ToLower())
                    {
                        case "y":
                            Console.WriteLine("Excellent. " + name + " shall be added into the database.");
                            validInput = true;
                            break;
                        case "n":
                            Console.WriteLine("I see. Retrying...");
                            validInput = false;
                            break;
                        default:
                            Console.WriteLine("Invalid input. Retrying...");
                            validInput = false;
                            break;
                    }
                }

                newDiet.Name = name;

                db.DietPlans.InsertOnSubmit(newDiet);
                db.SubmitChanges();

                newDiet = db.DietPlans.Where(m => m.Name == name).FirstOrDefault();

                return newDiet.DietPlanId;
            }

            foreach (var diet in dietPlans)
            {
                Console.Write(diet.DietPlanId+": "+diet.Name+" ");
                if (int.Parse(input) == diet.DietPlanId)
                {
                    return diet.DietPlanId;
                }
                else
                {
                    UserInterface.DisplayUserOptions("Input not recognized please try again.");
                    return GetDietPlanId();
                }
            }

            Console.WriteLine("Error detected. Returning first ID value from diet plans.");
            return 1;
        }

        internal static int GetCategoryId()
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            var categories = db.Categories;
            string name = null;

            Console.WriteLine("Current animal categories listed are: ");
            foreach (var category in categories)
            {
                Console.Write(category.Name + " ");
            }

            Console.WriteLine("Type out the category your animal belongs to.");
            Console.WriteLine("If your animal does not have a category, you may add one by typing in 'add'.");

            string input = Console.ReadLine();
            bool validInput = false;

            if (input.ToLower() == "add")
            {
                Category newCategory = new Category();

                while(!validInput)
                {
                    Console.WriteLine("You have selected to add an animal category. What is its name?");
                    name = Console.ReadLine();
                    Console.WriteLine("I see, so the name of the category is " + name + " y/n?");

                    string confirmInput = Console.ReadLine();
                    switch (confirmInput.ToLower())
                    {
                        case "y":
                            Console.WriteLine("Excellent. " + name + " shall be added into the database.");
                            validInput = true;
                            break;
                        case "n":
                            Console.WriteLine("I see. Retrying...");
                            validInput = false;
                            break;
                        default:
                            Console.WriteLine("Invalid input. Retrying...");
                            validInput = false;
                            break;
                    }
                }

                newCategory.Name = name;

                db.Categories.InsertOnSubmit(newCategory);
                db.SubmitChanges();

                newCategory = db.Categories.Where(m => m.Name == name).FirstOrDefault();

                return newCategory.CategoryId;
            }

            foreach (var category in categories)
            {
                Console.Write(category + " ");
                if (input.ToLower() == category.Name.ToString().ToLower())
                {
                    return category.CategoryId;
                }
                else
                {
                    UserInterface.DisplayUserOptions("Input not recognized please try again.");
                    return GetCategoryId();
                }
            }

            Console.WriteLine("Error detected. Returning first ID value from categories.");
            return 1;
        }
    }
}