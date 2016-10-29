using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieStore.Domain.Abstract;
using MovieStore.Domain.Entities;
using MovieStore.WebUI.Controllers;
using MovieStore.WebUI.HtmlHelpers;
using MovieStore.WebUI.Models;
using System.Web.Mvc;
using System;

namespace MovieStore.UnitTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
        }

        [TestMethod]
        public void Can_Paginate() {
            // Arrange
            Mock<IMovieRepository> mock = new Mock<IMovieRepository>();

            mock.Setup(m => m.Movies).Returns(new Movie[] {
                new Movie {MovieID = 1, Name = "Movie 1"},
                new Movie {MovieID = 2, Name = "Movie 2"},
                new Movie {MovieID = 3, Name = "Movie 3"},
                new Movie {MovieID = 4, Name = "Movie 4"},
                new Movie {MovieID = 5, Name = "Movie 5"}
            });

            MovieController controller = new MovieController(mock.Object);
            controller.PageSize = 3;
            
            // Act
            MovieListViewModel result = (MovieListViewModel)controller.List(null, 2).Model;

            // Assert
            Movie[] movieArray = result.Movies.ToArray();
            Assert.IsTrue(movieArray.Length == 2);
            Assert.AreEqual(movieArray[0].Name, "Movie 4");
            Assert.AreEqual(movieArray[1].Name, "Movie 5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links() {
            // Arrange - define an HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper htmlHelper = null;
            
            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;
            
            // Act
            MvcHtmlString result = htmlHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
            result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model() {
            // Arrange
            Mock<IMovieRepository> mock = new Mock<IMovieRepository>();
            mock.Setup(m => m.Movies).Returns(new Movie[] {
                new Movie {MovieID = 1, Name = "Movie 1"},
                new Movie {MovieID = 2, Name = "Movie 2"},
                new Movie {MovieID = 3, Name = "Movie 3"},
                new Movie {MovieID = 4, Name = "Movie 4"},
                new Movie {MovieID = 5, Name = "Movie 5"}
            });

            // Arrange
            MovieController controller = new MovieController(mock.Object);
            controller.PageSize = 3;

            // Act
            MovieListViewModel result = (MovieListViewModel)controller.List(null, 2).Model;
            
            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Movies() {
            // Arrange
            // - create the mock repository
            Mock<IMovieRepository> mock = new Mock<IMovieRepository>();
            mock.Setup(m => m.Movies).Returns(new Movie[] {
                new Movie {MovieID = 1, Name = "Movie 1", Category = "Actie" },
                new Movie {MovieID = 2, Name = "Movie 2", Category = "Horror" },
                new Movie {MovieID = 3, Name = "Movie 3", Category = "Thriller" },
                new Movie {MovieID = 4, Name = "Movie 4", Category = "Actie" },
                new Movie {MovieID = 5, Name = "Movie 5", Category = "Drama" }
            });

            // Arrange - create a controller and make the page size 3 items
            MovieController controller = new MovieController(mock.Object);
            controller.PageSize = 3;
            
            // Action
            Movie[] result = ((MovieListViewModel)controller.List("Actie", 1).Model).Movies.ToArray();
            
            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "Movie 1" && result[0].Category == "Actie");
            Assert.IsTrue(result[1].Name == "Movie 4" && result[1].Category == "Actie");
        }

        [TestMethod]
        public void Can_Create_Categories() {
            // Arrange
            // - create the mock repository
            Mock<IMovieRepository> mock = new Mock<IMovieRepository>();
            mock.Setup(m => m.Movies).Returns(new Movie[] {
                new Movie {MovieID = 1, Name = "Movie 1", Category = "Actie" },
                new Movie {MovieID = 2, Name = "Movie 2", Category = "Horror" },
                new Movie {MovieID = 3, Name = "Movie 3", Category = "Thriller" },
                new Movie {MovieID = 4, Name = "Movie 4", Category = "Actie" },
                new Movie {MovieID = 5, Name = "Movie 5", Category = "Drama" }
            });
            // Arrange - create the controller
            NavController target = new NavController(mock.Object);
            
            // Act = get the set of categories
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();
            
            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Actie");
            Assert.AreEqual(results[1], "Horror");
            Assert.AreEqual(results[2], "Thriller");
        }

        [TestMethod]
        public void Indicates_Selected_Category() {
            // Arrange
            // - create the mock repository
            Mock<IMovieRepository> mock = new Mock<IMovieRepository>();

            mock.Setup(m => m.Movies).Returns(new Movie[] {
                new Movie {MovieID = 1, Name = "Movie 1", Category = "Actie" },
                new Movie {MovieID = 2, Name = "Movie 2", Category = "Horror" },
             });

            // Arrange - create the controller
            NavController target = new NavController(mock.Object);
            
            // Arrange - define the category to selected
            string categoryToSelect = "Actie";
            
            // Action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Movie_Count() {
            // Arrange
            // - create the mock repository
            Mock<IMovieRepository> mock = new Mock<IMovieRepository>();
            mock.Setup(m => m.Movies).Returns(new Movie[] {
                new Movie {MovieID = 1, Name = "Movie 1", Category = "Actie" },
                new Movie {MovieID = 2, Name = "Movie 2", Category = "Horror" },
                new Movie {MovieID = 3, Name = "Movie 3", Category = "Thriller" },
                new Movie {MovieID = 4, Name = "Movie 4", Category = "Actie" },
                new Movie {MovieID = 5, Name = "Movie 5", Category = "Drama" }
                });
        
            // Arrange - create a controller and make the page size 3 items
            MovieController target = new MovieController(mock.Object);
            target.PageSize = 3;
        
            // Action - test the Movie counts for different categories
            int res1 = ((MovieListViewModel)target.List("Actie").Model).PagingInfo.TotalItems;
            int res2 = ((MovieListViewModel)target.List("Horror").Model).PagingInfo.TotalItems;
            int res3 = ((MovieListViewModel)target.List("Drama").Model).PagingInfo.TotalItems;
            int resAll = ((MovieListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            // Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 1);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
