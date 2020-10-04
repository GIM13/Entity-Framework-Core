namespace VaporStore.DataProcessor
{
	using System;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
            var genres = context.Genres
                      .ToArray()
                      .OrderByDescending(x => x.Id)
                      .Select(x => new
                      {
                          Id = x.Id,
                          Genre = x.Name,
                          Games = x.Games
                                   .Where(y => y.Purchases.Any())
                                   .Select(z => new
                                   {
                                       Id =z.Id,
                                       Title = z.Name,
                                       Developer = z.Developer.Name,
                                       Tags = string.Join(",",z.GameTags),
                                       Players = z.Purchases.Count()
                                   })
                                   .OrderByDescending(p => p.Players)
                                   .ThenBy(x => x.Id)
                                   .ToArray()
                      })
                      .OrderByDescending(x => x.Games.Sum(y => y.Players))
                      .ToArray();

            return JsonConvert.SerializeObject(genres, Formatting.Indented);
        }

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			throw new NotImplementedException();
		}
	}
}