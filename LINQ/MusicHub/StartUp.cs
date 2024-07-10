namespace MusicHub
{
    using System;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Console.WriteLine(ExportAlbumsInfo(context, 9));
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }



        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumInfo = context.Producers
                .Include(x => x.Albums).ThenInclude(a => a.Songs).ThenInclude(s => s.Writer)
                .First(x => x.Id == producerId)
                .Albums.Select(x => new
                {
                    AlbumName = x.Name,
                    x.ReleaseDate,
                    ProducerName = x.Producer.Name,
                    AlbumSongs = x.Songs.Select(x => new
                    {
                        SongName =x.Name,
                        SongPrice = x.Price,
                        SongWriterName = x.Writer.Name
                    })
                       .OrderByDescending(x => x.SongName)
                       .ThenBy(x => x.SongWriterName),
                    TotalAlbumPrice = x.Price
                }).OrderByDescending(x => x.TotalAlbumPrice).AsEnumerable();


            
            StringBuilder sb = new();

            foreach (var album in albumInfo)
            {
                sb
                    .AppendLine($"-AlbumName: {album.AlbumName}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy")}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine("-Songs:");  

                
                int counter = 1;
                foreach (var song in album.AlbumSongs)
                {
                    sb
                        .AppendLine($"---#{counter++}")
                        .AppendLine($"---SongName: {song.SongName}")
                        .AppendLine($"---Price: {song.SongPrice:f2}")
                        .AppendLine($"---Writer: {song.SongWriterName}");
                }
                

                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }



        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Include(s => s.SongPerformers).ThenInclude(sp => sp.Performer)
                .Include(s => s.Writer)
                .Include(s => s.Album).ThenInclude(a => a.Producer)
                .ToList()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    PerformerFullName = s.SongPerformers
                        .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                        .OrderBy(name => name)
                        .ToList(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c"),
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToList();


            StringBuilder sb = new StringBuilder();

            int counter = 1;

            foreach (var song in songs)
            {
                sb
                    .AppendLine($"-Song #{counter++}")
                    .AppendLine($"---SongName: {song.Name}")
                    .AppendLine($"---Writer: {song.WriterName}");

                foreach (var performer in song.PerformerFullName)
                {
                    sb.AppendLine($"---Performer: {performer}");
                }

                sb
                    .AppendLine($"---AlbumProducer: {song.AlbumProducer}")
                    .AppendLine($"---Duration: {song.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
