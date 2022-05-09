using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Extensions;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductRepository productRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository,
                                     IProductRepository productRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
        }
        [HttpGet]
        [Route("{userid}/GetItems")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetItems(int userId)
        {
            try
            {
                var cartItems = await this.shoppingCartRepository.GetItems(userId);

                if (cartItems == null)
                {
                    return NoContent();
                }
                var products = await this.productRepository.GetItems();

                if (products == null)
                {
                    throw new Exception("No product exists");
                }
                var cartItemsDto = cartItems.ConvertToDto(products);

                return Ok(cartItemsDto);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<CartItemDto>> GetItem(int id)
        {
            try
            {
                var cartItem = await this.shoppingCartRepository.GetItem(id);

                if (cartItem == null)
                {
                    return NotFound();
                }
                var product = await this.productRepository.GetItem(cartItem.ProductId);

                if (product == null)
                {
                    return NotFound();
                }
                var cartItemDto = cartItem.ConvertToDto(product);

                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
