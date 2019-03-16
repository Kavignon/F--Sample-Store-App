module CustomerCart

open System

open ItemDomain 
open customerAccount
open Common

type PaymentMethod = 
    | Visa of cardOwnerName: string * cardNumer: string * expirationDate: DateTime * cardSecurityCode: int * wasCloned: bool
    | Debit of cardOwnerName: string * cardNumber: string * BankName: string * wasCloned: bool

    member x.isCardValid =
        // Some complex valid going behind the scenes...
        match x with 
        | Visa(_, _, exprDate, _, wasCloned) -> DateTime.Now < exprDate && not wasCloned
        | Debit(_, _, _, wasCloned) -> not wasCloned

    member x.cardOwnerName = 
        match x with 
        | Visa (ownerName, _, _, _, _) -> ownerName
        | Debit (ownerName, _, _, _) -> ownerName

type ShoppingCart = {
    CustomerInfo: CustomerAccount;
    SelectedPaymentMethod: PaymentMethod
    SelectedItems: Map<StoreProduct, int> option
}
with 
    member x.isCartEmpty = x.SelectedItems |> Option.toList |> List.isEmpty

    member x.getCartSubtotal = 
        match x.SelectedItems with 
        | None -> 0.00m<usd>
        | Some storeProducts -> 
            (0.00m<usd>, storeProducts) 
            ||> Map.fold(fun accumulatedSubtotal product qty -> 
                accumulatedSubtotal + (product.ProductPrice * decimal qty))