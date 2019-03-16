module ItemDomain

open Common
open System

type SupportedLanguage = 
    | English
    | French
    | Spanish
    | German
    | Italian
    | Greek

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

type ComputerMouse = { DeviceDefinition: Device }

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
    | Mouse     of ComputerMouse
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
    IsNoiseCancelActive:    bool option
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