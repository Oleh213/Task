using System;
namespace Task.Models
{
	public class AddDogModel
	{
        public string Name { get; set; }

        public string Color { get; set; }

        public double Tail_lenght { get; set; }

        public double Weight { get; set; }
    }

    public enum AddDogStatus
    {
        DogNameExist,
        DogInfoInCorect,
        Success,
        UnknownError,
    }
}

