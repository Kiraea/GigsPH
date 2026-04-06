using GigPH.Domain;

namespace GigPH.Infrastructure.Seeds;
// can also make this to a service but hey for learning
public static class DatabaseSeeder 
{
    public static async Task SeedGenresAsync(AppDbContext dbContext)
    {
        if (dbContext.Genres.Any())
        {
            return;
        }

        var genres = new List<Genre>
        {
            // Rock & Alternative
            new Genre { Name = "Rock" },
            new Genre { Name = "Alternative Rock" },
            new Genre { Name = "Indie Rock" },
            new Genre { Name = "Classic Rock" },
            new Genre { Name = "Hard Rock" },
            new Genre { Name = "Punk Rock" },
            new Genre { Name = "Post-Punk" },
            new Genre { Name = "Grunge" },
            new Genre { Name = "Psychedelic Rock" },
            new Genre { Name = "Shoegaze" },
    
            // Pop
            new Genre { Name = "Pop" },
            new Genre { Name = "Indie Pop" },
            new Genre { Name = "Synthpop" },
            new Genre { Name = "Electropop" },
            new Genre { Name = "Dream Pop" },
            new Genre { Name = "Teen Pop" },
            new Genre { Name = "K-Pop" },
            new Genre { Name = "J-Pop" },
            new Genre { Name = "C-Pop" },
            new Genre { Name = "Britpop" },

            // Hip Hop & Rap
            new Genre { Name = "Hip Hop" },
            new Genre { Name = "Rap" },
            new Genre { Name = "Trap" },
            new Genre { Name = "Boom Bap" },
            new Genre { Name = "Lo-fi Hip Hop" },
            new Genre { Name = "Drill" },
            new Genre { Name = "Old School Hip Hop" },
            new Genre { Name = "Conscious Hip Hop" },
            new Genre { Name = "Alternative Hip Hop" },
            new Genre { Name = "UK Grime" },

            // R&B, Soul & Funk
            new Genre { Name = "R&B" },
            new Genre { Name = "Contemporary R&B" },
            new Genre { Name = "Soul" },
            new Genre { Name = "Neo Soul" },
            new Genre { Name = "Funk" },
            new Genre { Name = "Disco" },
            new Genre { Name = "Motown" },
            new Genre { Name = "Gospel" },
            new Genre { Name = "Quiet Storm" },
            new Genre { Name = "Doo-Wop" },

            // Electronic & Dance
            new Genre { Name = "Electronic" },
            new Genre { Name = "EDM" },
            new Genre { Name = "House" },
            new Genre { Name = "Techno" },
            new Genre { Name = "Trance" },
            new Genre { Name = "Dubstep" },
            new Genre { Name = "Drum and Bass" },
            new Genre { Name = "Ambient" },
            new Genre { Name = "Synthwave" },
            new Genre { Name = "Chillout" },

            // Metal
            new Genre { Name = "Metal" },
            new Genre { Name = "Heavy Metal" },
            new Genre { Name = "Death Metal" },
            new Genre { Name = "Black Metal" },
            new Genre { Name = "Doom Metal" },
            new Genre { Name = "Metalcore" },
            new Genre { Name = "Nu Metal" },
            new Genre { Name = "Power Metal" },
            new Genre { Name = "Symphonic Metal" },
            new Genre { Name = "Thrash Metal" },

            // Country & Folk
            new Genre { Name = "Country" },
            new Genre { Name = "Alt-Country" },
            new Genre { Name = "Bluegrass" },
            new Genre { Name = "Americana" },
            new Genre { Name = "Folk" },
            new Genre { Name = "Indie Folk" },
            new Genre { Name = "Singer-Songwriter" },
            new Genre { Name = "Acoustic" },
            new Genre { Name = "Celtic Folk" },
            new Genre { Name = "Contemporary Folk" },

            // Jazz & Blues
            new Genre { Name = "Jazz" },
            new Genre { Name = "Smooth Jazz" },
            new Genre { Name = "Bebop" },
            new Genre { Name = "Swing" },
            new Genre { Name = "Free Jazz" },
            new Genre { Name = "Blues" },
            new Genre { Name = "Delta Blues" },
            new Genre { Name = "Chicago Blues" },
            new Genre { Name = "Rhythm and Blues" },
            new Genre { Name = "Jazz Fusion" },

            // Latin & Caribbean
            new Genre { Name = "Latin" },
            new Genre { Name = "Reggaeton" },
            new Genre { Name = "Salsa" },
            new Genre { Name = "Bachata" },
            new Genre { Name = "Merengue" },
            new Genre { Name = "Cumbia" },
            new Genre { Name = "Bossa Nova" },
            new Genre { Name = "Reggae" },
            new Genre { Name = "Dancehall" },
            new Genre { Name = "Ska" },

            // Classical, World & Miscellaneous
            new Genre { Name = "Classical" },
            new Genre { Name = "Opera" },
            new Genre { Name = "Baroque" },
            new Genre { Name = "Romantic" },
            new Genre { Name = "Choral" },
            new Genre { Name = "World Music" },
            new Genre { Name = "Afrobeats" },
            new Genre { Name = "Flamenco" },
            new Genre { Name = "New Age" },
            new Genre { Name = "Experimental" }
        };
        await dbContext.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync();
    }

    public static async Task SeedInstruments( AppDbContext dbContext)
    {
        if (dbContext.Instruments.Any())
        {
            return;
        }
        var instruments = new List<Instrument>
        {
            new Instrument { Name = "Vocals" },
            new Instrument { Name = "Backing Vocals" },

            // Guitars & Strings
            new Instrument { Name = "Acoustic Guitar" },
            new Instrument { Name = "Electric Guitar" },
            new Instrument { Name = "Bass Guitar" },
            new Instrument { Name = "Ukulele" },
            new Instrument { Name = "Banjo" },
            new Instrument { Name = "Mandolin" },

            // Bowed Strings
            new Instrument { Name = "Violin / Fiddle" },
            new Instrument { Name = "Viola" },
            new Instrument { Name = "Cello" },
            new Instrument { Name = "Upright Bass" },

            // Keys
            new Instrument { Name = "Piano" },
            new Instrument { Name = "Keyboard / Synth" },
            new Instrument { Name = "Organ" },
            new Instrument { Name = "Accordion" },

            // Percussion (Simplified!)
            new Instrument { Name = "Drums" }, // Covers the whole kit
            new Instrument { Name = "Hand Percussion" }, // Covers tambourines, shakers, cowbells
            new Instrument { Name = "Bongos / Congas" },
            new Instrument { Name = "Cajon" },
            new Instrument { Name = "Mallet Percussion" }, // Xylophone, Marimba, etc.

            // Horns & Woodwinds
            new Instrument { Name = "Saxophone" },
            new Instrument { Name = "Trumpet" },
            new Instrument { Name = "Trombone" },
            new Instrument { Name = "Flute" },
            new Instrument { Name = "Clarinet" },
            new Instrument { Name = "Harmonica" },

            // Electronic / Production
            new Instrument { Name = "DJ / Turntables" },
            new Instrument { Name = "Drum Machine / Sampler" },
            new Instrument { Name = "MIDI Controller" }
        };
        await dbContext.AddRangeAsync(instruments);
        await dbContext.SaveChangesAsync();
    }
}