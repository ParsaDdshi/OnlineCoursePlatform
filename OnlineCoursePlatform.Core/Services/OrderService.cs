using Microsoft.EntityFrameworkCore;
using OnlineCoursePlatform.Core.DTOs.Order;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Context;
using OnlineCoursePlatform.DataLayer.Entities.Course;
using OnlineCoursePlatform.DataLayer.Entities.Order;
using OnlineCoursePlatform.DataLayer.Entities.User;
using OnlineCoursePlatform.DataLayer.Entities.Wallet;

namespace OnlineCoursePlatform.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly OCPContext _context;
        private readonly IUserService _userService;
        private readonly ICourseService _coueseService;
        public OrderService(OCPContext context, IUserService userService, ICourseService courseService)
        {
            _context = context;
            _userService = userService;
            _coueseService = courseService;
        }

        public void AddDiscount(Discount discount)
        {
            _context.Discounts.Add(discount);
            Save();
        }

        public void DeleteCourseFromOrder(int detailId)
        {
            OrderDetail detail = _context.OrderDetails.SingleOrDefault(d => d.DetailId == detailId);
            if (detail != null)
            {
                _context.OrderDetails.Remove(detail);
                Save();
            }
        }

        public void DeleteDiscount(int discountId)
        {
            Discount discount = _context.Discounts.Find(discountId);
            discount.IsDelete = true;
            Save();
        }

        public AdminDiscountViewModel GetAllDiscounts(int pageId = 1)
        {
            IQueryable <Discount> discounts = _context.Discounts;
            int take = 10;
            int skip = (pageId - 1) * take;

            AdminDiscountViewModel result = new AdminDiscountViewModel();
            result.PageCount = discounts.Count() / take;
            result.CurrentPage = pageId;
            result.Discounts = discounts.Skip(skip).Take(take).ToList();

            return result;
        }

        public AdminDiscountViewModel GetDeletedDiscounts(int pageId = 1) 
        {
            IQueryable<Discount> discounts = _context.Discounts.IgnoreQueryFilters().Where(d => d.IsDelete == true);
            int take = 10;
            int skip = (pageId - 1) * take;

            AdminDiscountViewModel result = new AdminDiscountViewModel();
            result.PageCount = discounts.Count() / take;
            result.CurrentPage = pageId;
            result.Discounts = discounts.Skip(skip).Take(take).ToList();

            return result;
        }

        public Discount GetDiscountById(int discountId) => _context.Discounts.Find(discountId);

        public DiscountCodeDates GetDiscountDates(int discountId) => _context.Discounts
            .Where(d => d.DiscountId == discountId)
            .Select(d => new DiscountCodeDates()
            {
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                DiscountCode = d.DiscountCode,
            }).Single();

        public Order GetOrderById(int orderId) => _context.Orders
            .Find(orderId);

        public Order GetOrderForUserPanel(string userName, int orderId)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            return _context.Orders.Include(o => o.OrderDetails)
                .ThenInclude(d => d.Course)
                .FirstOrDefault(o => o.UserId == userId && o.OrderId == orderId);
        }

        public List<Order> GetUserOrdersByUserName(string userName) => _context.Orders
            .Where(o => o.UserId == _userService.GetUserIdByUserName(userName)).ToList();

        public int InsertOrder(string userName, int courseId)
        {
            int userId = _userService.GetUserIdByUserName(userName);

            Order order = _context.Orders
                .FirstOrDefault(o => o.UserId == userId && !o.IsFinished);

            Course course = _coueseService.GetCourseById(courseId);
            if (order == null)
            {
                order = new Order()
                {
                    CreateDate = DateTime.Now,
                    IsFinished = false,
                    UserId = userId,
                    OrderSum = course.CoursePrice,
                    OrderDetails = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            CourseId  = courseId,
                            Count = 1,
                            Price = course.CoursePrice,                              
                        }
                    },
                };
                _context.Orders.Add(order);
                Save();
            }
            else
            {
                OrderDetail detail = _context.OrderDetails
                    .FirstOrDefault(d => d.OrderId == order.OrderId && d.CourseId == courseId);

                if(detail == null)
                {
                    detail = new OrderDetail()
                    {
                        CourseId = courseId,
                        Count = 1,
                        OrderId = order.OrderId,
                        Price = course.CoursePrice,
                    };
                    _context.OrderDetails.Add(detail);
                    Save();
                    UpdateOrderSum(order.OrderId);
                }
            }
            return order.OrderId;
        }

        public bool IsDiscountCodeExist(string discountCode) => _context.Discounts
            .Any(d => d.DiscountCode == discountCode);

        public bool IsUserInCourse(string userName, int courseId) => _context.UserCourses
            .Any(u => u.UserId == _userService.GetUserIdByUserName(userName) && u.CourseId == courseId);

        public bool PayOrder(int orderId, string userName)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            Order order = _context.Orders.Include(o => o.OrderDetails)
                .ThenInclude(d => d.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null || order.IsFinished)
                return false;

            if(_userService.UserWalletBalance(userName) >= order.OrderSum)
            {
                order.IsFinished = true;
                _userService.AddWallet(new Wallet()
                {
                    Amount = order.OrderSum,
                    CreateDate = DateTime.Now,
                    Description = "فاکتور شماره #" + order.OrderId,
                    IsPay = true,
                    TypeId = 2,
                    UserId = userId              
                });

                foreach(OrderDetail detail in order.OrderDetails)
                {
                    _context.UserCourses.Add(new UserCourse()
                    {
                        CourseId = detail.CourseId,
                        UserId = userId,                        
                    });
                }
                Save();

                return true;
            }
            return false;
        }

        public void Save() => _context.SaveChanges();

        public void UpdateDiscount(Discount discount)
        {
            _context.Discounts.Update(discount);
            Save();
        }

        public void UpdateOrderSum(int ordreId)
        {
            Order order = _context.Orders.Find(ordreId);
            order.OrderSum = _context.OrderDetails
                .Where(d => d.OrderId == ordreId).Sum(d => d.Price);
            Save();
        }

        public DiscountUseType UseDiscount(int orderId, string discountCode)
        {
            Discount discount = _context.Discounts
                .SingleOrDefault(d => d.DiscountCode == discountCode);

            Order order = GetOrderById(orderId);

            if (order.OrderSum == 0)
                return DiscountUseType.EmptyOrder;

            if (discount == null)
                return DiscountUseType.NotFound;

            if (discount.StartDate != null && discount.StartDate >= DateTime.Now)
                return DiscountUseType.ExpiredDate;

            if (discount.EndDate != null && discount.EndDate < DateTime.Now)
                return DiscountUseType.ExpiredDate;

            if (discount.UsableCount != null && discount.UsableCount < 1)
                return DiscountUseType.Finished;

            int orderSum = _context.OrderDetails.Where(d => d.OrderId == orderId)
                .Sum(d => d.Price);

            if (_context.UserDiscountCodes.Any(u => u.UserId == order.UserId && u.DiscountId == discount.DiscountId))
                return DiscountUseType.UsedByUser;

            if (orderSum != order.OrderSum)
                return DiscountUseType.UsedInOrder;

            int discountAmount = (order.OrderSum * discount.Percent) / 100;
            order.OrderSum = order.OrderSum - discountAmount;

            if (discount.UsableCount != null)
                discount.UsableCount--;

            _context.UserDiscountCodes.Add(new UserDiscountCode()
            {
                UserId = order.UserId,
                DiscountId = discount.DiscountId
            });
            Save();
            return DiscountUseType.Success;
        }
    }
}
