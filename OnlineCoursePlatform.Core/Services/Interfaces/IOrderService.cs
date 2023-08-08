using OnlineCoursePlatform.Core.DTOs.Order;
using OnlineCoursePlatform.DataLayer.Entities.Order;

namespace OnlineCoursePlatform.Core.Services.Interfaces
{
    public interface IOrderService
    {
        #region Order

        int InsertOrder(string userName, int courseId);
        void UpdateOrderSum(int ordreId);
        Order GetOrderForUserPanel(string userName, int orderId);
        bool PayOrder(int orderId, string userName);
        List<Order> GetUserOrdersByUserName(string userName);
        Order GetOrderById(int orderId);
        bool IsUserInCourse(string userName, int courseId);
        void DeleteCourseFromOrder(int detailId);

        #endregion

        #region Discount

        DiscountUseType UseDiscount(int orderId, string discountCode);
        void AddDiscount(Discount discount);
        AdminDiscountViewModel GetAllDiscounts(int pageId = 1);
        Discount GetDiscountById(int discountId);
        void UpdateDiscount(Discount discount);
        DiscountCodeDates GetDiscountDates(int discountId);
        bool IsDiscountCodeExist(string discountCode);
        void DeleteDiscount(int discountId);
        AdminDiscountViewModel GetDeletedDiscounts(int pageId = 1);

        #endregion

        void Save();
    }
}
