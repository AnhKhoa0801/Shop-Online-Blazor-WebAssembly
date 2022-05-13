using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ShoppingCartBase : ComponentBase
    {
        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }
        public List<CartItemDto> ShoppingCartItems { get; set; }
        public string ErrorMesssage { get; set; }
        public int TotalQty { get; set; }
        public string TotalPrice { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ShoppingCartItems = await ShoppingCartService.GetItems(HardCoded.UserId);
                CartSummary();
            }
            catch (Exception)
            {

                throw;
            }
        }


        protected async Task DeleteCartItem_Click(int id)
        {
            try
            {
                var cartItemDto = await ShoppingCartService.DeleteItem(id);
                RemoveCartItem(id);
                CartSummary();
            }
            catch (Exception)
            {

                throw;
            }
        }



        private async Task MakeUpdate(int id, bool visible)
        {
            await JS.InvokeVoidAsync("MakeUpdateQtyVisible", id, visible);
        }

        protected async Task UpdateQty_Input(int id)
        {
            await MakeUpdate(id, true);
        }


        protected async Task UpdateQtyCartItem_Click(int id, int qty)
        {
            try
            {
                if (qty > 0)
                {
                    var cartItemQtyUpdateDto = new CartItemQtyUpdateDto
                    {
                        CartItemId = id,
                        Qty = qty
                    };
                    var updatedItem = await this.ShoppingCartService.UpdateQty(cartItemQtyUpdateDto);
                    UpdateItemTotalPrice(updatedItem);
                    CartSummary();
                    await MakeUpdate(id, false);
                }
                else
                {
                    var item = this.ShoppingCartItems.FirstOrDefault(i => i.Id == id);

                    if (item != null)
                    {
                        item.Qty = 1;
                        item.TotalPrice = item.Price;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void UpdateItemTotalPrice(CartItemDto cartItemDto)
        {
            var item = GetCartItem(cartItemDto.Id);
            if (item != null)
            {
                item.TotalPrice = item.Price * item.Qty;
            }
        }

        private void CartSummary()
        {
            SetTotalPrice();
            SetTotalQuantity();
        }

        private void SetTotalPrice()
        {
            TotalPrice = this.ShoppingCartItems.Sum(i => i.TotalPrice).ToString("C");
        }
        private void SetTotalQuantity()
        {
            TotalQty = this.ShoppingCartItems.Sum(i => i.Qty);
        }

        private CartItemDto GetCartItem(int id)
        {
            return ShoppingCartItems.FirstOrDefault(x => x.Id == id);
        }

        private void RemoveCartItem(int id)
        {
            var cartItemDto = GetCartItem(id);

            ShoppingCartItems.Remove(cartItemDto);
        }

    }
}
