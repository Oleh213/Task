using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Task.BusinessLogic;
using Task.Context;
using Task.DBContext;
using Task.Interfaces;
using Task.Models;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Options;
using System;
using Task.DTO;
using Newtonsoft.Json;

namespace TaskTesting;

public class DogsActionsTests : IDisposable
{

    private readonly DogsActionsBL _dogsActionsBL;
    private readonly DogsContext _dogsContext;
    public DogsActionsTests()
    {
        var options = new DbContextOptionsBuilder<DogsContext>()
            .UseInMemoryDatabase(databaseName: "TaskDataBase")
            .Options;
        _dogsContext = new DogsContext(options);
        _dogsContext.Dogs.Add(new Dog { Name = "Bony", Color = "red", Tail_lenght = 100, Weight = 10 });
        _dogsContext.Dogs.Add(new Dog { Name = "Bini", Color = "green", Tail_lenght = 75, Weight = 15 });
        _dogsContext.Dogs.Add(new Dog { Name = "Alex", Color = "black", Tail_lenght = 30, Weight = 43 });
        _dogsContext.Dogs.Add(new Dog { Name = "Kalya", Color = "grey & black", Tail_lenght = 34, Weight = 34 });
        _dogsContext.Dogs.Add(new Dog { Name = "Jora", Color = "white", Tail_lenght = 86, Weight = 27 });
        _dogsContext.SaveChanges();

        _dogsActionsBL = new DogsActionsBL(_dogsContext);
    }
    public void Dispose()
    {
        var context = _dogsContext.Dogs.ToList();
        _dogsContext.Dogs.RemoveRange(context);
        _dogsContext.SaveChanges();

    }

    [Fact]
    public async void Get_Dogs_Withot_Query_Params()
    {
        var model = new GetDogsModel();
        var result = await _dogsActionsBL.GetDogs(model);

        Assert.Equal(5,result.Count());
    }

    [Fact]
    public async void Check_Dog_Name()
    {
        var result = await _dogsActionsBL.CheckDogName("Jora");

        Assert.True(result);
    }

    [Fact]
    public async void Check_Input_Params()
    {
        var model = new AddDogModel { Name = "Gory", Color = "yellow"};
        var result = _dogsActionsBL.CheckDogModel(model);

        Assert.False(result);
    }

    [Fact]
    public async void Get_Dogs_With_Sorting_Attribute()
    {
        var model = new GetDogsModel{ Attribute = "weight", Order = "desc"};
        var result = await _dogsActionsBL.GetDogs(model);
        var dogs = _dogsContext.Dogs.ToList().OrderByDescending(x=> x.Weight).ToList();

        var dogDTOs = dogs.Select(dog => new DogDTO
        {
            Name = dog.Name,
            Color = dog.Color,
            Tail_lenght = dog.Tail_lenght,
            Weight = dog.Weight
        }).ToList();

        var dogDTOsToJson = JsonConvert.SerializeObject(dogDTOs);
        var resultToJson = JsonConvert.SerializeObject(result.ToList());
        Assert.Equal(dogDTOsToJson, resultToJson);

    }

    [Fact]
    public async void Add_Dog_To_Data_Base()
    {
        var model = new AddDogModel { Name = "Geri", Color = "black & white", Tail_lenght = 30, Weight = 100 };

        var result = await _dogsActionsBL.AddDogToDataBase(model);

        Assert.True(result);
    }

    [Fact]
    public async void Add_Dogs_Success()
    {
        var model = new AddDogModel { Name = "Gory", Color = "yellow", Tail_lenght = 30, Weight = 10 };
        var result = await _dogsActionsBL.AddDogAction(model);

        Assert.Equal(AddDogStatus.Success, result);
    }

    [Fact]
    public async void Get_Dogs_With_Paginations()
    {
        var model = new GetDogsModel { PageNumber = 1, PageSize = 2, };
        var result = await _dogsActionsBL.GetDogs(model);
        var dogDTOs = _dogsActionsBL.TransferToDTO(_dogsContext.Dogs.Take(2).ToList());

        var dogDTOsToJson = JsonConvert.SerializeObject(dogDTOs);
        var resultToJson = JsonConvert.SerializeObject(result.ToList());
        Assert.Equal(dogDTOsToJson, resultToJson);
    }

    [Fact]
    public async void Add_Dogs_With_Negative_Wight_And_Tail()
    {
        var model = new AddDogModel { Name = "Bony", Color = "green", Tail_lenght = -10, Weight = -10 };
        var result = await _dogsActionsBL.AddDogAction(model);

        Assert.Equal(AddDogStatus.DogInfoInCorect, result);
    }

    [Fact]
    public async void Add_Dogs_With_Exist_Name()
    {
        var model = new AddDogModel { Name = "Bony", Color = "green", Tail_lenght = 32, Weight = 10 };
        var result = await _dogsActionsBL.AddDogAction(model);

        Assert.Equal(AddDogStatus.DogNameExist, result);
    }
}
