using System;
using Microsoft.EntityFrameworkCore;
using Task.Context;
using Task.DTO;
using Task.Models;

namespace Task.Interfaces
{
	public interface IDogsActionsBL
	{
        Task<IEnumerable<DogDTO>> GetDogs(GetDogsModel model);

        Task<bool> AddDogToDataBase(AddDogModel model);

        Task<bool> CheckDogName(string dogName);

        bool CheckDogModel(AddDogModel model);

        Task<AddDogStatus> AddDogAction(AddDogModel dogModel);
    }
}

