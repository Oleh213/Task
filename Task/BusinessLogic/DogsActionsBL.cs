using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Task.Context;
using Task.DBContext;
using Task.DTO;
using Task.Interfaces;
using Task.Models;

namespace Task.BusinessLogic
{
    public class DogsActionsBL : IDogsActionsBL
    {
        private DogsContext _context;

        public DogsActionsBL(DogsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DogDTO>> GetDogs(GetDogsModel model)
        {
            var _dogs = await _context.Dogs.ToListAsync();

            var dogsResult = new List<Dog>();

            var dogDTOs = new List<DogDTO>();

            if (!string.IsNullOrEmpty(model.Attribute) && !string.IsNullOrEmpty(model.Order))
            {
                var propertyInfo = typeof(Dog).GetProperty(model.Attribute, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    if (model.Order.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    {
                        dogsResult = _dogs.OrderByDescending(dog => propertyInfo.GetValue(dog, null)).ToList();
                    }
                    else
                    {
                        dogsResult = _dogs.OrderBy(dog => propertyInfo.GetValue(dog, null)).ToList();
                    }
                }
            }
            else
            {
                dogsResult = _dogs;
            }

            if (model.PageNumber != null && model.PageSize != null)
            {
                var pageNumber = model.PageNumber.GetValueOrDefault();
                var pageSize = model.PageSize.GetValueOrDefault();
                dogsResult = dogsResult.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            if (!dogsResult.IsNullOrEmpty())
            {
                dogDTOs = TransferToDTO(dogsResult);
            }

            return dogDTOs;
        }

        public List<DogDTO> TransferToDTO(List<Dog> dogs)
        {
            var dogDTOs = dogs.Select(dog => new DogDTO
            {
                Name = dog.Name,
                Color = dog.Color,
                Tail_lenght = dog.Tail_lenght,
                Weight = dog.Weight
            }).ToList();

            return dogDTOs;
        }

        public async Task<bool> AddDogToDataBase(AddDogModel model)
        {
            var dog = new Dog
            {
                Name = model.Name,
                Weight = model.Weight,
                Color = model.Color,
                Tail_lenght = model.Tail_lenght,
            };

            var respons = await _context.Dogs.AddAsync(dog);

            await _context.SaveChangesAsync();

            return _context.Dogs.Contains(dog);
        }
        public bool CheckDogModel(AddDogModel model)
        {
            return !string.IsNullOrEmpty(model.Color) && !string.IsNullOrEmpty(model.Name) && model.Tail_lenght > 0 && model.Weight > 0;
        }

        public async Task<bool> CheckDogName(string dogName)
            => await _context.Dogs.AnyAsync(x => x.Name == dogName);

        public async Task<AddDogStatus> AddDogAction(AddDogModel model)
        {
            if (!CheckDogModel(model))
            {
                return AddDogStatus.DogInfoInCorect;
            }
            if(await CheckDogName(model.Name))
            {
                return AddDogStatus.DogNameExist;
            }
            if (await AddDogToDataBase(model))
            {
                return AddDogStatus.Success;
            }
            else
            {
                return AddDogStatus.UnknownError;
            }
        }
    }
}

