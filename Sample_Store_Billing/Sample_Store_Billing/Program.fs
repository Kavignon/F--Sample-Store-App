open Common
open ItemDomain
open customerAccount
open CustomerCart
open Billing

open System

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

    let storeProducts = loadStoreProducts productsFromDatabase

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
