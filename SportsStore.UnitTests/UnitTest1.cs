using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Тестирование стредсва разбиения на страницы.
        /// Создаем имитированное хранилище, внедрив его в конструктор класса ProductController 
        /// и вызвав метод List(), чтобы запросить конкретную страницу.
        /// Затем полученый объект Product сравниваем с ожидаемым от тестовых данных.
        /// </summary>
        [TestMethod]
        public void Can_Paginate()
        {
            //ОРГАНИЗАЦИЯ
            //Создание имитации объекта с помощью Moq
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            //Настройка реализации
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID =1, Name = "P1"},
                new Product{ProductID =2, Name = "P2"},
                new Product{ProductID =3, Name = "P3"},
                new Product{ProductID =4, Name = "P4"},
                new Product{ProductID =5, Name = "P5"}
            });
            //Создаем экземляр класса 
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //ДЕЙСТВИЕ
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;

            //УТВЕРЖДЕНИЕ
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        /// <summary>
        /// Тестирование вывода спомогательного метода с ипользованием латерального строкового значения.
        /// </summary>
        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //ОРГАНИЗАЦИЯ
            //Определяем вспомогательный метод HTML(Это необходимо для применения расширяющего метода)
            HtmlHelper myHelper = null;

            //Создаем данные PagingInfo
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItem = 28,
                ItemPerPage = 10
            };

            //Настраиваем делегат с помощью лямбда-выражения
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //ДЕЙСТВИЕ
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //УТВЕРЖДЕНИЕ
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //ОРГАНИЗАЦИЯ
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID =1, Name = "P1"},
                new Product{ProductID =2, Name = "P2"},
                new Product{ProductID =3, Name = "P3"},
                new Product{ProductID =4, Name = "P4"},
                new Product{ProductID =5, Name = "P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //ДЕЙСТВИЕ
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;

            //УТВЕРЖДЕНИЕ
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItem, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }
    }
}
