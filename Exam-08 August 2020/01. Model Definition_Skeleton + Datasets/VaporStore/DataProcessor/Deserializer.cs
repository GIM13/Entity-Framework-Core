namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            var result = string.Empty;

            var gamesDTO = JsonConvert.DeserializeObject<ImportGamesDTO[]>(jsonString);

            var games = new List<Game>();

            foreach (var gameDTO in gamesDTO)
            {
                if (!IsValid(gameDTO))
                {
                    result += "Invalid Data" + Environment.NewLine;
                    continue;
                }

                DateTime releaseDate;
                var validReleaseDate = DateTime.TryParseExact(gameDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate);

                if (!validReleaseDate)
                {
                    result += "Invalid Data" + Environment.NewLine;
                    continue;
                }

                var developers = context.Developers.Select(x => x.Name);
                var developer = !developers.Contains(gameDTO.Developer) ? new Developer { Name = gameDTO.Developer } : context.Developers.FirstOrDefault(x => x.Name == gameDTO.Developer);

                var genres = context.Developers.Select(x => x.Name);
                var genre = !genres.Contains(gameDTO.Genre) ? new Genre { Name = gameDTO.Genre } : context.Genres.FirstOrDefault(x => x.Name == gameDTO.Genre);

                var tags = context.GameTags
                    .Select(x => x.Tag.Name);

                var game = new Game
                {
                    Name = gameDTO.Name,
                    Price = gameDTO.Price,
                    ReleaseDate = releaseDate,
                    Developer = developer,
                    Genre = genre,
                    GameTags = gameDTO.Tags
                    .Select(x => !tags.Contains(x) 
                                 ? new GameTag { Tag = new Tag { Name = x} } 
                                 : context.GameTags.FirstOrDefault(y => y.Tag.Name == x ))
                    .ToArray()
                };

                if (!IsValid(game) && game.GameTags.Any(x => x.Tag != null))
                {
                    result += "Invalid Data" + Environment.NewLine;
                    continue;
                }

                games.Add(game);

                result += $"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags" + Environment.NewLine;
            };

            context.Games.AddRange(games);

            context.SaveChanges();

            return result.Trim();
        }

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			throw new NotImplementedException();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			throw new NotImplementedException();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}