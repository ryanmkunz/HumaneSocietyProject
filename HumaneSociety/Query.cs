﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
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
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
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
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
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
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            Employee employeeInQuery = new Employee(); 
            switch (crudOperation)
            {
                case "create":
                    AddEmployee(employeeInQuery);
                    break;
                case "read":
                    List<string> info = new List<string> { employee.FirstName, employee.LastName, employee.UserName, employee.Email };
                    UserInterface.DisplayUserOptions(info);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                default:
                    break;
            }
        }      
        
        //Employee CRUD Operations
        internal static void AddEmployee(Employee employee)
        {
            Employee newEmployee = new Employee();
            newEmployee.EmployeeId = employee.EmployeeId;
            newEmployee.FirstName = employee.FirstName;
            newEmployee.LastName = employee.LastName;
            newEmployee.UserName = employee.UserName;
            newEmployee.Password = employee.Password;
            newEmployee.EmployeeNumber = employee.EmployeeNumber;
            newEmployee.Email = employee.Email;
            db.Employees.InsertOnSubmit(newEmployee);
            db.SubmitChanges();
        }

        internal static void UpdateEmployee(Employee employee)
        {
            db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).ToList().ForEach(e => e.FirstName = employee.FirstName);
            db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).ToList().ForEach(e => e.LastName = employee.LastName);
            db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).ToList().ForEach(e => e.EmployeeNumber = employee.EmployeeNumber);
            db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).ToList().ForEach(e => e.Email = employee.Email);
            db.SubmitChanges();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            Animal newAnimal = new Animal();
            newAnimal.AnimalId = animal.AnimalId;
            newAnimal.Name = animal.Name;
            newAnimal.Weight = animal.Weight;
            newAnimal.Age = animal.Age;
            newAnimal.Demeanor = animal.Demeanor;
            newAnimal.KidFriendly = animal.KidFriendly;
            newAnimal.PetFriendly = animal.PetFriendly;
            newAnimal.Gender = animal.Gender;
            newAnimal.AdoptionStatus = animal.AdoptionStatus;
            newAnimal.CategoryId = animal.CategoryId;
            newAnimal.DietPlanId = animal.DietPlanId;
            newAnimal.EmployeeId = animal.EmployeeId;
            db.Animals.InsertOnSubmit(newAnimal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.Where(a => a.AnimalId == id).First();
        }       

        internal static void UpdateAnimal(Animal animal, Dictionary<int, string> updates)
        {
            //dictionary has values 1-8 for keys, and the string of the updated value
            animal.Category.Name = updates[1];
            animal.Name = updates[2];
            animal.Age = int.Parse(updates[2]);
            animal.Demeanor = updates[4];
            animal.KidFriendly = bool.Parse(updates[5]);
            animal.PetFriendly = bool.Parse(updates[6]);
            animal.Weight = int.Parse(updates[7]);
            animal.AnimalId = int.Parse(updates[8]);
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        // Got special permission from Nevin to change return type
        internal static List<Animal> SearchForAnimalByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            List<Animal> newContainer = db.Animals.ToList();
            
            if (updates[1] != null)
            {
                newContainer.RemoveAll(a => a.Category.Name != updates[1]);                
            }
            if (updates[2] != null)
            {
                newContainer.RemoveAll(a => a.Name == updates[2]);
            }
            if (updates[3] != null)
            {
                newContainer.RemoveAll(a => a.Age == int.Parse(updates[3]));
            }
            if (updates[4] != null)
            {
                newContainer.RemoveAll(a => a.Demeanor == updates[4]);
            }
            if (updates[5] != null)
            {
                newContainer.RemoveAll(a => a.KidFriendly == bool.Parse(updates[5]));
            }
            if (updates[6] != null)
            {
                newContainer.RemoveAll(a => a.PetFriendly == bool.Parse(updates[6]));
            }
            if (updates[7] != null)
            {
                newContainer.RemoveAll(a => a.Weight == int.Parse(updates[7]));
            }
            if (updates[8] != null)
            {
                newContainer.RemoveAll(a => a.AnimalId == int.Parse(updates[8]));
            }
            return newContainer;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            throw new NotImplementedException();
        }
        
        internal static Room GetRoom(int animalId)
        {
            throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}