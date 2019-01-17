using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static Action<Employee, HumaneSocietyDBDataContext> employeeCRUD;

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

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress1, string streetAddress2, int zipCode, int stateId)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress1 && a.AddressLine2 == streetAddress2 && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress1;
                newAddress.AddressLine2 = streetAddress2;
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
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.AddressLine2 == clientAddress.AddressLine2 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.AddressLine2 = clientAddress.AddressLine2;
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

        internal static void UpdateAdoption(bool verification, Adoption adoption)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();
            Animal adoptedAnimal = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).Single();
            Adoption adoptionFromDb = db.Adoptions.Where(d => d.AdoptionId == adoption.AdoptionId).Single();

            if (verification)
            {
                adoptionFromDb.ApprovalStatus = "approved";
                adoptedAnimal.AdoptionStatus = "Adopted";
            }
            else
            {
                adoptionFromDb.ApprovalStatus = "pending";
                adoptedAnimal.AdoptionStatus = "Unadopted";
            }

            adoptionFromDb.PaymentCollected = verification;

            db.SubmitChanges();
        }

        internal static void RunEmployeeQueries(Employee employee, string query)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            switch (query)
            {
                case "create":
                    employeeCRUD = addEmp;
                    employeeCRUD(employee, db);
                    break;
                case "read":
                    employeeCRUD = readEmp;
                    employeeCRUD(employee, db);
                    break;
                case "update":
                    employeeCRUD = updateEmp;
                    employeeCRUD(employee, db);
                    break;
                case "delete":
                    employeeCRUD = deleteEmp;
                    employeeCRUD(employee, db);
                    break;
                default:
                    employeeCRUD = readEmp;
                    employeeCRUD(employee, db);
                    break;
            }
        }

        //CRUD methods
        internal static void addEmp(Employee selectedEmployee, HumaneSocietyDBDataContext db)
        {
            Employee newEmployee = new Employee();

            newEmployee.FirstName = selectedEmployee.FirstName;
            newEmployee.LastName = selectedEmployee.LastName;
            newEmployee.UserName = selectedEmployee.UserName;
            newEmployee.Password = selectedEmployee.Password;
            newEmployee.EmployeeNumber = selectedEmployee.EmployeeNumber;
            newEmployee.Email = selectedEmployee.Email;

            db.Employees.InsertOnSubmit(newEmployee);
            db.SubmitChanges();
        }
        internal static void readEmp(Employee selectedEmployee, HumaneSocietyDBDataContext db)
        {
            Employee checkedEmployee = db.Employees.Where(e => e.EmployeeNumber == selectedEmployee.EmployeeNumber).Single();

            Console.WriteLine("Employee ID:" + checkedEmployee.EmployeeId);

            Console.WriteLine("Employee First Name:" + checkedEmployee.FirstName);
            Console.WriteLine("Employee Last Name:" + checkedEmployee.LastName);
            Console.WriteLine("Employee User Name:" + checkedEmployee.UserName);
            Console.WriteLine("Employee Password:" + checkedEmployee.Password);
            Console.WriteLine("Employee Email:" + checkedEmployee.Email);
            Console.WriteLine("Employee Number:" + checkedEmployee.EmployeeNumber);
            Console.ReadLine();
        }
        internal static void updateEmp(Employee selectedEmployee, HumaneSocietyDBDataContext db)
        {
            Console.WriteLine("");
            Console.WriteLine("Enter the ID# of the employee this information is being used for.");

            Employee[] employeeList = db.Employees.ToArray();
            foreach(Employee employee in employeeList)
            {
                Console.WriteLine(employee.EmployeeId +". " + employee.FirstName + " " +employee.LastName);
            }

            int employeeInt = int.Parse(Console.ReadLine());

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == selectedEmployee.EmployeeId).Single();

            employeeFromDb.FirstName = selectedEmployee.FirstName;
            employeeFromDb.LastName = selectedEmployee.LastName;
            employeeFromDb.Email = selectedEmployee.Email;

            db.SubmitChanges();
        }
        internal static void deleteEmp(Employee selectedEmployee, HumaneSocietyDBDataContext db)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeNumber == selectedEmployee.EmployeeNumber && e.LastName == selectedEmployee.LastName).Single();

            db.Employees.DeleteOnSubmit(employeeFromDb);
            db.SubmitChanges();
        }
        //End of CRUD methods

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

        internal static void Adopt(Animal animal, Client client)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            Adoption pendingAdoption = new Adoption();

            pendingAdoption.ClientId = client.ClientId;
            pendingAdoption.AnimalId = animal.AnimalId;

            pendingAdoption.ApprovalStatus = "pending";
            pendingAdoption.AdoptionFee = 50;
            pendingAdoption.PaymentCollected = false;

            animal.AdoptionStatus = "Adopted";

            db.Adoptions.InsertOnSubmit(pendingAdoption);
            db.SubmitChanges();
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
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            var selectedShots = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId);

            AnimalShot[] shotList = selectedShots.ToArray();

            return shotList;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDBDataContext  db = new HumaneSocietyDBDataContext();

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return (employeeWithUserName != null);
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
            newShot.DateReceived = DateTime.Today;

            db.AnimalShots.InsertOnSubmit(newShot);

            db.SubmitChanges();
        }

        internal static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();

            if (updates.ContainsKey(1))
            {
                animalFromDb.Category = db.Categories.Where(c => c.Name == updates[1]).FirstOrDefault();
            }
            if (updates.ContainsKey(2))
            {
                animalFromDb.Name = updates[2];
            }
            if (updates.ContainsKey(3))
            {
                animalFromDb.Age = int.Parse(updates[3]);
            }
            if (updates.ContainsKey(4))
            {
                animalFromDb.Demeanor = updates[4];
            }
            if (updates.ContainsKey(5))
            {
                if (updates[5].ToLower() == "yes" || updates[5].ToLower() == "y")
                {
                    animalFromDb.KidFriendly = true;
                }
                else
                {
                    animalFromDb.KidFriendly = false;
                }  
            }
            if (updates.ContainsKey(6))
            {
                if (updates[6].ToLower() == "yes" || updates[6].ToLower() == "y")
                {
                    animalFromDb.PetFriendly = true;
                }
                else
                {
                    animalFromDb.PetFriendly = false;
                }
            }
            if (updates.ContainsKey(7))
            {
                animalFromDb.Weight = int.Parse(updates[7]);
            }

            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal selectedAnimal)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();
            Animal deletedAnimal = db.Animals.Where(a => a.AnimalId == selectedAnimal.AnimalId).Single();

            db.Animals.DeleteOnSubmit(deletedAnimal);
            db.SubmitChanges();
        }

        internal static void AddAnimal(Animal selectedAnimal)
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();
            Animal newAnimal = new Animal();

            newAnimal.Name = selectedAnimal.Name;
            newAnimal.CategoryId = selectedAnimal.CategoryId;
            newAnimal.Weight = selectedAnimal.Weight;
            newAnimal.Age = selectedAnimal.Age;
            newAnimal.Demeanor = selectedAnimal.Demeanor;
            newAnimal.KidFriendly = selectedAnimal.KidFriendly;
            newAnimal.PetFriendly = selectedAnimal.PetFriendly;
            newAnimal.DietPlanId = selectedAnimal.DietPlanId;
            newAnimal.Gender = selectedAnimal.Gender;
            newAnimal.AdoptionStatus = "Unadopted";
            newAnimal.EmployeeId = selectedAnimal.EmployeeId;

            db.Animals.InsertOnSubmit(newAnimal);
            db.SubmitChanges();

            db.Rooms.Where(r => r.AnimalId == null).FirstOrDefault().AnimalId = newAnimal.AnimalId;

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
                Console.WriteLine("Diet #: "+diet.DietPlanId+", Name: "+diet.Name+", food type: "+diet.FoodType+", Amount of food in cups: "+diet.FoodAmountInCups);
            }

            Console.WriteLine("Type out the number associated with your animal's diet.");
            Console.WriteLine("If your animal does not have a diet plan, you may add one by typing in 'add'.");
            Console.WriteLine("You may also update an existing diet plan by typing in 'update'.");

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
                newDiet.FoodType = type;
                newDiet.FoodAmountInCups = amount;

                db.DietPlans.InsertOnSubmit(newDiet);
                db.SubmitChanges();

                newDiet = db.DietPlans.Where(m => m.Name == name).Single();
                Console.WriteLine("Diet successfully added. This diet shall now be used.");
                return newDiet.DietPlanId;
            }
            else if (input.ToLower() == "update")
            {
                
                Console.WriteLine("You have selected to update an existing diet plan. Type out its number below.");
                DietPlan selectedDiet = db.DietPlans.Where(m => m.DietPlanId == int.Parse(Console.ReadLine())).Single();

                while (!validInput)
                {
                    Console.WriteLine("What is its name?");
                    name = Console.ReadLine();
                    Console.WriteLine("What food type is it?");
                    type = Console.ReadLine();
                    Console.WriteLine("How many cups are in one serving?");
                    amount = int.Parse(Console.ReadLine());

                    Console.WriteLine("I see, so the name of the category is " + name + ", ");
                    Console.WriteLine("the type of food is " + type + ", ");
                    Console.WriteLine("and the amount of cups in one serving is " + amount.ToString() + ".");

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

                selectedDiet.Name = name;
                selectedDiet.FoodType = type;
                selectedDiet.FoodAmountInCups = amount;

                db.SubmitChanges();

                Console.WriteLine("Diet successfully updated. This diet shall now be used.");
                return selectedDiet.DietPlanId;
            }

            foreach (var diet in dietPlans)
            {
                if (int.Parse(input) == diet.DietPlanId)
                {
                    return diet.DietPlanId;
                }
            }

            UserInterface.DisplayUserOptions("Input not recognized please try again.");
            return GetDietPlanId();
        }

        internal static int GetCategoryId()
        {
            HumaneSocietyDBDataContext db = new HumaneSocietyDBDataContext();

            var categories = db.Categories;
            string name = null;

            Console.WriteLine("Current animal categories listed are: ");
            foreach (var category in categories)
            {
                Console.Write(category.CategoryId + ". " + category.Name + " ");
                Console.WriteLine();
            }

            Console.WriteLine("Type out the the category your animal belongs to.");
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
                if (input.ToLower() == category.Name.ToLower())
                {
                    return category.CategoryId;
                }
            }

            UserInterface.DisplayUserOptions("Input not recognized please try again.");
            return GetCategoryId();
        }
    }
}