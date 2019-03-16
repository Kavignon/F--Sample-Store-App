module customerAccount

type PersonalName = {
    FirstName:      string;
    MiddleName:     string option
    LastName:       string
}
with 
    member x.Fullname = 
        match x.MiddleName with 
        | Some middle -> [x.FirstName; middle; x.LastName] |> List.fold (+) ""
        | None -> [x.FirstName; x.LastName] |>  List.fold (+) ""

type PostalCode =  PostalCode of string   
type City = City of string
type Province = Province of string

// Address
type StreetAddress = {
    CivicNumber:int; 
    StreetName:string;
    PostalCode: PostalCode;
}

type CanadaAddress = {
    Street: StreetAddress;
    City: City;
    Province: Province
}

// Email
type Email = Email of string

// Phone
type CountryPrefix = CountryPrefix of int
type Phone = { CountryPrefix:CountryPrefix; LocalNumber:string }

type Contact = {
    CustomerName: PersonalName;
    BillingAddress: CanadaAddress;
    ShippingAddress: CanadaAddress option;
    IsShippingToBillingAddress: bool;
    Email: Email;
    Phone: Phone option;
}

// Put it all together into a CustomerAccount type
type CustomerAccountId  = AccountId of string

// override equality and deny comparison
[<CustomEquality; NoComparison>]
type CustomerAccount = 
  {
    AccountId: CustomerAccountId
    ContactInfo: Contact
    }

    override this.Equals(other) =
        match other with
        | :? CustomerAccount as otherCust -> (this.AccountId = otherCust.AccountId)
        | _ -> false

    override this.GetHashCode() = hash this.AccountId 