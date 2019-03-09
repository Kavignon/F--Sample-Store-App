open Common
open ItemDomain
open customerAccount
open CustomerCart
open Billing

open System

let productList = (productDataFromDb.GetSample())
    let rec loadStoreHeadphones  storeProductList (headphoneData:   productDataFromDb.Headphone list) =
        match headphoneData with
        | [] -> storeProductList
        | headphone :: remains -> 
            let brand = 
                match headphone.Manufacturer.Name with 
                | Prefix "So" _ -> Sony
                | Prefix "Bo" _-> Bose
                | _ -> NonSupported
            
            let fit = 
                match headphone.Fit.Value with 
                | Prefix "In" _ -> InEar
                | Prefix "On" _ -> OnEar
                | _ -> OverEar
            
            let color = 
                match headphone.Color.Value with 
                | BlackColor -> Black
                | WhiteColor -> White 
                | _ -> Red

            let product = [ WirelessHeadphones { Manufacturer = brand; ModelName = headphone.Model.Name; Fit = fit; Dimensions = { Heigth = headphone.Heigth.Value * 1.00m<cm>; Width = headphone.Width.Value * 1.00m<cm>; Depth = Some (headphone.Depth.Value * 1.00m<cm>) }; BatteryLife = Some(castToHour headphone.BatteryLife.Value); ReleaseDate = DateTime(headphone.ReleaseDate.Value, 1, 1); Pricing = { Price = headphone.Price.Value |> castToUsd; ShippingWeigth = headphone.Weigth.Value|> castToKg }; Color = color; AreWireless = headphone.IsWireless.Value; IsNoiseCancelActive = Some(headphone.IsNoiseCancelled.Value)            
            } ]

            let storeProductList = storeProductList |> List.append product
            (storeProductList, remains) ||> loadStoreHeadphones 

    let rec loadStoreBooks storeProductList (bookData:  productDataFromDb.Book list) =
        match bookData with 
        | [] -> storeProductList
        | book :: remains -> 
            let bookCategory = 
                match book.Category.Value with
                | Prefix "Fan" _ -> Some Fantasy
                | Prefix "Comp" _ & Suffix "Sciences" _ -> Some ``Computer Science``
                | _ -> None

            let product = [ Book { Name = book.Name.Value; Category = bookCategory; AuthorName = book.AuthorName.Value; PageCount = book.PageCount.Value; PricingDetails = { Price = book.Price.Value |> castToUsd; ShippingWeigth = book.ShippingWeight.Value |> castToKg }; ReviewAverage = book.ReviewAverage.Value |> castToStarReview } ]
            let storeProductList = storeProductList |> List.append product
            (storeProductList, remains)
            ||> loadStoreBooks 

    let rec loadStoreComputers storeProductList (computerData: productDataFromDb.Computer list) = 
        match computerData with 
        | [] -> storeProductList
        | computer :: remains -> loadStoreComputers storeProductList remains

    let loadStoreProducts (storeData: productDataFromDb.Items) products = 
       (products, Array.toList storeData.Headphones) 
        ||> loadStoreHeadphones 
        |> loadStoreBooks <| Array.toList(storeData.Books)
        |> loadStoreComputers <| Array.toList(storeData.Computers)

        //|> loadStoreTelevisions <| Array.toList(storeData.Televisions)
        //|> loadGameConsoles <| Array.toList(storeData.GameConsoles)

[<EntryPoint>]
let main argv = 
    let contactInfo = { 
        CustomerName = { FirstName = "Kevin"; MiddleName = Some "Olivier"; LastName = "Avignon"}
        BillingAddress = { 
            Street = { CivicNumber = 1234; StreetName = "First Ave."; PostalCode = PostalCode("A1B 2C3")}
            City = City("Montreal")
            Province = Province("Quebec")
        }
        ShippingAddress = None
        IsShippingToBillingAddress = true
        Email = Email("kevin.o.avignon@gmail.com")
        Phone = None
    }

    let customerInformation = { 
        AccountId = AccountId("Kevin-Avignon-123456789")
        ContactInfo = contactInfo
    }

    // TODO: Load customers from CustomerDb.xml

    // TODO: Let customer buy items from the store. 

    // TODO: Until purchased, items are still part of the inventory

    // TODO: User cannot purchased more than the given limit of an item, must be capped. 

    // TODO: Move away from statically creating the shopping cart and use the items provided by the "LIVE" customer

    // TODO: Recursive program until 
        // No more items of any sort
        // User chooses 'Q' and quits the program.

    let storeProducts = loadStoreProducts productList []

    let bloodMirrorNovel = { Name = "Blood Mirror"; AuthorName = "Brent Weeks"; Category = Some Fantasy; PageCount = 555; PricingDetails = { Price = 34.50m<usd>; ShippingWeigth = 2.2<kg>}; ReviewAverage = 4.25<stars> }
    let boseQuietComfort = { Manufacturer = Bose; ModelName = "QuietComfort 35 Series II"; Fit = OverEar; Dimensions = { Heigth = 3.20m<cm>; Width = 6.70m<cm>; Depth = Some 7.31m<cm> }; BatteryLife = Some (30y<hr>); ReleaseDate = DateTime(2017,1,1); Pricing = { Price = 349.00m<usd>; ShippingWeigth = 2.2<kg>}; Color = Black; AreWireless = true; IsNoiseCancelActive = Some true }

    let selectedCustomerProducts = 
       Map.empty
        .Add(Book(bloodMirrorNovel), 5)
        .Add(WirelessHeadphones(boseQuietComfort), 3)
        //.Add(Television(), 2)
       
    let customerCart = { 
        CustomerInfo    = customerInformation
        SelectedPaymentMethod   = Visa("Kevin Olivier Avignon", "1234 2345 3456 5782", DateTime.MaxValue, 123, false)
        SelectedItems   = Some selectedCustomerProducts
    }

    processCartCheckout customerCart

    0 // return an integer exit code
