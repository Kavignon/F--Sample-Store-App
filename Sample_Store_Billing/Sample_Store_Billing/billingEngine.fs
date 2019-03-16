module Billing

open CustomerCart
open Common
open ItemDomain

let shippingFee = 15.00m<usd>

type OrderSummary = {
    ItemsOrdered: int
    StartingSubtotal: decimal<usd>
    IsShippingFree: bool
    ShippingFee: decimal<usd> option
    Taxes: decimal<usd>
}
with 
    member x.orderTotal =
        let billTotal = x.StartingSubtotal + x.Taxes
        if  x.IsShippingFree then billTotal
        else 
            match x.ShippingFee with 
            | Some fee -> fee + billTotal
            | None -> billTotal

let getCartItemTotal selectedProducts = 
    (0, selectedProducts) ||> Map.fold(fun accTotal _ count -> accTotal + count)

let getOrderTotalTaxes (selectedProducts: Map<StoreProduct, int>) = 
    (0.00m<usd>, selectedProducts) ||> Map.fold(fun accTaxes product count -> 
        let decimalCount = count |> decimal 
        let baseTaxes = 0.15m
        let environmentalTaxes = 0.05m

        match product with 
        | Book b -> accTaxes + (baseTaxes * b.Details.Price * decimalCount)
        | WirelessHeadphones wh -> 
            accTaxes + (baseTaxes * wh.Details.Price * decimalCount) + (environmentalTaxes * wh.Details.Price * decimalCount)
        | Television t ->
            accTaxes + (baseTaxes * t.ProductDetails.Price * decimalCount) + (environmentalTaxes * t.ProductDetails.Price * decimalCount)
        | _ -> accTaxes
    )

let processCartCheckout (cart: ShoppingCart) =
    if not cart.SelectedPaymentMethod.isCardValid  && cart.CustomerInfo.ContactInfo.CustomerName.Fullname = cart.SelectedPaymentMethod.cardOwnerName then
        printfn "Cannot proceed to checkout with invalid payment method."
    else 
        match cart.SelectedItems with 
        | None -> printf "Your cart is empty. We can't process it for now."
        | Some items -> 
            let itemCount = getCartItemTotal items
            let totalTaxes = getOrderTotalTaxes items
            let subTotal = cart.getCartSubtotal
            let orderSummary = { ItemsOrdered = itemCount; StartingSubtotal = subTotal; ShippingFee = Some shippingFee; Taxes = totalTaxes; IsShippingFree = if subTotal > 35.00m<usd> then true else false }
            
            printfn "Order summary: %A" orderSummary
            printfn "Order Total %M" orderSummary.orderTotal