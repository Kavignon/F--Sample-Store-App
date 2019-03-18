module ItemDomain

open Common
open System

type DeviceDisplay = 
    | StandardDefinition    of int<pixel>
    | EnhanceDefinition     of int<pixel>
    | HighDefinition        of int<pixel>
    | UltraHighDefinition   of int<pixel>

type OperatingSystem =
    | WindowsXp
    | Windows7
    | Windows8
    | Windows10
    | MacOS
    | Linux
    | XboxOne
    | Playstation4
    | Switch

type CableConnection =
    | USB1
    | USB2
    | USB3
    | USBC
    | HDMI
    | MiniHDMI
    | PowerAdapter

type Device = { 
    ProductDetails:     CommonProductInformation
    ModelNumber:        string
    IsWireless:         bool
    SupportedOS:        OperatingSystem list
    HardwareInterfaces: CableConnection list
    Resolution:         DeviceDisplay
}

type Keyboard = {
    DeviceDefinition:   Device
    IsMechanical:       bool
    IsGamingKeyboard:   bool
    KeyCount:           byte
}

type FightingPad = { 
    DeviceDescription:      Device;
    AreBatteriesRequired:   bool
    HasProgrammableButtons: bool
}

type DeviceInput = 
    | Keyboard  of Keyboard
    | Gamepad   of FightingPad

type IntelProcessorSeries = 
    | IntelCorei3
    | IntelCorei5
    | IntelCorei7
    | IntelCorei9

type AmdProcessorSeries =
    | Ryden
    | Athlon
    | AthlonII
    | ASeries
    | ESeries
    | FSeries

type ProcessorSeries =
    | Intel     of IntelProcessorSeries
    | AMD       of AmdProcessorSeries

type DDR = 
    | DDR2
    | DDR3
    | DDR4

type CPU = {
    Details:            CommonProductInformation
    CoreCount:          byte
    Series:             ProcessorSeries
    ProcessorSpeed:     float<Ghz>
    OverclockedSpeed:   float<Ghz>
    Wattage:            int<watt>
    YearModel:          DateTime
}

type Computer = {
    Details:                CommonProductInformation
    Resolution:             DeviceDisplay    
    Cpu:                    CPU
    Ram:                    int<Mb>
    CacheMemory:            int<Mb> option
    DdrRam:                 DDR option
    RunningOperatingSystem: OperatingSystem
    DeviceInputs:           DeviceInput list
}

type GameConsole = { 
    Hardware:               Computer
    SupportedResolutions:   DeviceDisplay list
    Inputs:                 CableConnection list
    IsHandHandledDevice:    bool 
    MaxControllerSupported: byte
}

type BookCategory = 
    | Fantasy
    | ``Computer Science``
    | ``Graphic Novel``

type BookFormat = 
    | Paperback
    | Hardcover
    | Pdf
    | KindleVersion

let bookCategory (book: ProductDb.Book)= 
    match book.Category.Value with
    | Prefix "Fan" _ -> Some Fantasy
    | Prefix "Comp" _ & Suffix "Sciences" _ -> Some ``Computer Science``
    | _ -> None

type Book = { 
    AuthorName:     string
    Format:         BookFormat
    Summary:        string
    Details:        CommonProductInformation
    Category:       BookCategory
    PageCount:      int
    ISBN:           string
    Language:       SupportedLanguage
    ReleasedDate:   DateTime
}

type HeadphoneFit = 
    | ``In ear`` 
    | ``On ear`` 
    | ``Over ear``

type HeadphoneProduct = {
    Details:                CommonProductInformation
    Fit:                    HeadphoneFit
    BatteryLife:            sbyte<hr> option
    ReleaseDate:            DateTime;
    AreWireless:            bool
    IsNoiseCancelActive:    bool
}

type DbProductUtils =
    static member getFitFromHeadphones (h: ProductDb.Headphone) = 
        match h.Fit.Value with 
            | Prefix "In" _ -> ``In ear``
            | Prefix "On" _ -> ``On ear``
            | _ -> ``Over ear``

    static member getColorFromDbProduct colorString = 
       match colorString with 
        | "Red" -> Some Red 
        | "Black" -> Some Black 
        | "White" -> Some White 
        | "Gray" -> Some Gray
        | "Blue" -> Some Blue
        | "Green" -> Some Green 
        | _ -> None 
        |> Option.defaultValue ProductColor.NotSupportedByStore

    static member getBrandFromDbProduct brandString = 
        match brandString with 
        | "Toshiba" -> Some Toshiba
        | "Sony" -> Some Sony
        | "Microsoft" -> Some Microsoft
        | "Intel" -> Some Brand.Intel
        | "AMD" -> Some Brand.AMD
        | "Nintendo" -> Some Nintendo
        | "Bose" -> Some Bose
        | "Asus" -> Some Asus 
        | "Apple" -> Some Apple 
        | _ -> None 
        |> Option.defaultValue Brand.NotSupportedByStore


let getProductInfoFromProvider providerType = 
        match providerType with 
        | Headphones dbHeadphones -> 
            {
                Name = dbHeadphones.Model.Name
                Weight = dbHeadphones.Weigth.Value |> castToKg
                ShippingWeight = dbHeadphones.Weigth.Value |> castToKg // Must be updated to ShipingWeight 
                AverageReviews = 4.2<star> // Update xml with AverageReview field
                Dimensions = {
                    Heigth = dbHeadphones.Heigth.Value |> castToCm
                    Width = dbHeadphones.Width.Value |> castToCm
                    Depth = Some (dbHeadphones.Depth.Value |> castToCm)
                }
                Price = dbHeadphones.Price.Value |> castToUsd
                Color = dbHeadphones.Color.Value |> DbProductUtils.getColorFromDbProduct
                Brand = dbHeadphones.Manufacturer.Name |> DbProductUtils.getBrandFromDbProduct
            }
        | ReadingMaterial dbBook -> 
            {
                Name = dbBook.Name.Value
                Weight = dbBook.ShippingWeight.Value |> castToKg // Add weight field for books...
                ShippingWeight = dbBook.ShippingWeight.Value |> castToKg
                AverageReviews = dbBook.ReviewAverage.Value |> castToStarReview
                Dimensions = {
                    // Add dimensions to book definition
                    Heigth = 1.00m<cm>
                    Width = 1.00m<cm>
                    Depth = Some 1.00m<cm>
                }
                Price = dbBook.Price.Value |> castToUsd
                Color = Red // Provide book color in definition 
                Brand = Toshiba //Waiting for up book publisher companies in definition
            }
        | Computer computerDb -> 
            {
                Name = computerDb.Model.Series + " " + computerDb.Model.Number
                Weight = computerDb.Weight.Value |> castToKg 
                ShippingWeight = computerDb.Weight.Value |> castToKg // Add shipping weight field for computers...
                AverageReviews = 4.5<star> // reviews
                Dimensions = {
                    // Add dimensions to book definition
                    Heigth = computerDb.Height.Value |> castToCm
                    Width = computerDb.Height.Value |> castToCm
                    Depth = Some (computerDb.Height.Value |> castToCm)
                }
                Price = computerDb.Price.Value |> castToUsd
                Color = computerDb.Color.Value |>DbProductUtils.getColorFromDbProduct // Provide book color in definition 
                Brand = computerDb.Manufacturer |> DbProductUtils.getBrandFromDbProduct
            }

[<StructuralComparison; StructuralEquality>]
type StoreProduct =
    | Book                  of novel:       Book
    | WirelessHeadphones    of headphones:  HeadphoneProduct
    | Television            of television:  Device
    | Laptop                of laptop:      Computer
    | GameConsole           of console:     GameConsole
with 
    member x.ProductPrice = 
        match x with 
        | Book b -> b.Details.Price
        | WirelessHeadphones wh -> wh.Details.Price
        | Television t -> t.ProductDetails.Price
        | Laptop l -> l.Details.Price
        | GameConsole gc -> gc.Hardware.Details.Price

let getStoreHeadphones storeProductList =
    Array.map(fun h -> 
        let headphonesInfo = Headphones(h) |> getProductInfoFromProvider 
        let product = { 
            Details = headphonesInfo
            Fit = DbProductUtils.getFitFromHeadphones h
            BatteryLife = Some(castToHour h.BatteryLife.Value)
            ReleaseDate = h.ReleaseDate.Value 
            AreWireless = h.IsWireless.Value
            IsNoiseCancelActive = h.IsNoiseCancelled.Value
        }
        WirelessHeadphones(product)
    )
    >> Array.append storeProductList

let getStoreBooks storeProductList = 
    Array.map(fun b ->
        let bookInfo = ReadingMaterial(b) |> getProductInfoFromProvider 
        let product = {
            Details = bookInfo
            AuthorName = b.AuthorName.Value
            Format = KindleVersion // Missing format from XML document
            ISBN = "" // Missing ISBN from XML document 
            Summary = "" // Missing Summary from XML document
            PageCount = b.PageCount.Value
            Language = English
            Category = Fantasy // Missing function to convert from the XML document 
            ReleasedDate = DateTime.Now // Missing release date from XML document
        }
        Book(product)
    )
    >> Array.append storeProductList

let getStoreComputers storeProductList = 
    Array.map(fun computer ->
        let computerInfo = Computer(computer) |> getProductInfoFromProvider
        let computerCpu = {
            Details = computerInfo // Need function to retrieve it from XmlProvider<...>.Computer
            CoreCount = 4uy // Need function to retrieve it from XmlProvider<...>.Computer
            Series = Intel(IntelCorei7) // Need function to retrieve it from XmlProvider<...>.Computer
            ProcessorSpeed = 3.2<Ghz> // Need function to retrieve it from XmlProvider<...>.Computer
            OverclockedSpeed = 5.2<Ghz> // Need function to retrieve it from XmlProvider<...>.Computer
            Wattage = 80<watt> // Need function to retrieve it from XmlProvider<...>.Computer
            YearModel = DateTime.Today // Need function to retrieve it from XmlProvider<...>.Computer + Should be int, not DateTime
        }
        let product = {
            Details = computerInfo
            Resolution = HighDefinition(1920<pixel>)
            Cpu = computerCpu
            Ram = 8192<Mb> // Need conversion for RAM 
            CacheMemory = None // Need to specify field in computer + function to retrieve it from the generated type
            DdrRam = None 
            RunningOperatingSystem = Windows10
            DeviceInputs = []
        }
        Laptop(product)
    )
    >> Array.append storeProductList

let loadStoreProducts (storeItems: ProductDb.Items)  = 
    [||] 
    |> getStoreHeadphones <| storeItems.Headphones 
    |> getStoreBooks <| storeItems.Books
    |> getStoreComputers <| storeItems.Computers