using System;
using System.Linq.Expressions;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Chinook.Repository.Model;

namespace Chinook.Repository.Filter {
    public class ArtistFilter : RepositoryFilter<Model.Artist> {
        public ArtistFilter(Expression<Func<Model.Artist, bool>> criteria, IRepositoryInclude<Model.Artist> includes = null) : base(criteria, includes) {
        }

        /// <summary>
        /// Filter: Get Artist by name 
        /// </summary>
        public static ArtistFilter ByName(string artistName) {
            return new ArtistFilter(g => g.Name == artistName);
        }

        /// <summary>
        /// Filter: Get Artist by name, including Albums, albumTracks, trackGenre and trackMediaType
        /// </summary>
        public static ArtistFilter Discography(string artistName) {
            var includes = new RepositoryInclude<Artist>();
            includes.Include(a => a.Album).ThenInclude(alb => alb.Track).ThenInclude(t => t.Genre);
            includes.Include(a => a.Album).ThenInclude(alb => alb.Track).ThenInclude(t => t.MediaType);
            return new ArtistFilter(g => g.Name == artistName, includes);
        }
    }
}