using LemmyWeb.Controllers;
using LemmyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace LemmyWeb.Pages
{
    public class StatsModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;
        public LemmNannyStats Stats { get; set; } = new LemmNannyStats();
        public StatsModel(IMemoryCache cache)
        {
            _memoryCache = cache;
        }
        public void OnGet()
        {
            var stats = new LemmNannyStats();
            if (!_memoryCache.TryGetValue(Webhook.STATS_KEY, out stats))
            {
                stats = new LemmNannyStats();
            }
            else
            {
                Stats = stats!;
            }
        }
    }
}
