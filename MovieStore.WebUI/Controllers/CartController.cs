using System.Linq;
using System.Web.Mvc;
using MovieStore.Domain.Abstract;
using MovieStore.Domain.Entities;
using MovieStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers {
    public class CartController : Controller {
        private IMovieRepository repository;
        private IOrderProcessor orderProcessor;

        public CartController(IMovieRepository repo, IOrderProcessor proc) {
            repository = repo;
            orderProcessor = proc;
        }

        public ViewResult Index(Cart cart, string returnUrl) {
            return View(new CartIndexViewModel {
                ReturnUrl = returnUrl,
                Cart = cart
            });
        }

        public RedirectToRouteResult AddToCart(Cart cart, int movieId, string returnUrl) {
            Movie movie = repository.Movies.FirstOrDefault(p => p.MovieID == movieId);

            if (movie != null) {
                cart.AddItem(movie, 1);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int movieId, string returnUrl) {
            Movie movie = repository.Movies.FirstOrDefault(p => p.MovieID == movieId);
            
            if (movie != null) {
                cart.RemoveLine(movie);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public PartialViewResult Summary(Cart cart) {
            return PartialView(cart);
        }

        public ViewResult Checkout() {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails) {
            if (cart.Lines.Count() == 0) {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }

            if (ModelState.IsValid) {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");

            } else {
                return View(shippingDetails);
            }
        }
    }
}