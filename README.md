# Gilded Rose Merchant Api

A starting point for an HTTP accessible Merchant API using ASP.Net Web API. Current operations

- Obtain an Inventory "feed"
- Query availability of Items
- Submit Orders

#### Data Format

`JSON` is lightweight (e.g. less "verbose" than `XML`) and versatile format. It is _relatively_ easier to parse, serialize/deserialize by clients. 

**Sample Request**

```
//Request header
POST /api/order/MID001
Content-Type: application/json
Authorization: Token eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.ey........
....

{
	"MerchantId": "MID001",
	"OrderItems": [{
		"QtyOrdered": 1,
		"Name": "Gilded Item 2",
		"Description": "Item 2 description",
		"Price": 199
	},
	{
		"QtyOrdered": 2,
		"Name": "Gilded Item 3",
		"Description": "Item 3 description",
		"Price": 299
	},
	{
		"QtyOrdered": 3,
		"Name": "Gilded Item 4",
		"Description": "Item 4 description",
		"Price": 399
	},
	{
		"QtyOrdered": 4,
		"Name": "Gilded Item 5",
		"Description": "Item 5 description",
		"Price": 499
	}],
	"TaxTotal": 0,
	"ShippingTotal": 0,
	"DiscountTotal": 0,
	"MerchantOrderReference": "1871",
	"OrderId": "18018",
	"OrderDate": 1456129268
}
```

**Sample Api Response**

```
//Response header
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Location: http://localhost:55774/api/order/MID001
...

{
	"transactionId": "1871-TID829",
	"submittedOrder": {
		"merchantId": "MID001",
		"orderItems": [{
			"qtyOrdered": 1,
			"name": "Gilded Item 2",
			"description": "Item 2 description",
			"price": 199
		},
		{
			"qtyOrdered": 2,
			"name": "Gild ed Item 3",
			"description": "Item 3 description",
			"price": 299
		},
		{
			"qtyOrdered": 3,
			"name": "Gilded Item 4",
			"description": "Item 4 description",
			"price": 399
		},
		{
			"qtyOrdered": 4,
			"name": "Gilded Item 5",
			"description": " Item 5 description",
			"price": 499
		}],
		"taxTotal": 0,
		"shippingTotal": 0,
		"discountTotal": 0,
		"merchantOrderReference": "1871",
		"orderId": "18018",
		"orderDate": 1456129268
	},
	"status": "Success",
	"message": "Successfully recieved 1871, dated 2/22/2016 12:21:08 AM, valued at 3990"
}
```

#### Authentication

`JWT` ([JSON Web Token](https://self-issued.info/docs/draft-ietf-oauth-json-web-token.html)) was chosen (over relatively simpler methods, such as `BASIC`) for added security and versatility. The protocol allows 2 parties (via a shared secret) to digitally sign tokens thereby uniquely identifying "issuer" and "audience", as well as the integrity of the contents of the message. While commonly used in `Headers`, it can just as easily be used in (larger) payloads (e.g. request body). Both authentication and message integrity are realized in one protocol. Expirations limit the lifetime of JWTs as well.

To authenticate to the API, Merchants will be provided a `Merchant Id` and `secret key` to generate a JWT Token used in HTTP Headers. 

Example:

`Authorization: Token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJpQXBwcyIsImlhdCI6MTQ..`

**A Note On Authentiation**

The API currently defines a `user` as _clients of the Api_ (merchants). In a real world scenario `users` can also mean _end users_ (of merchants). Depending on this definition and implementation details, different approaches are possible:

- Gilded Rose Maintains a directory for both types<br/>
A sample implementation could be providing an endpoint for _end user_ authentication, responding with a _token_ (Jwt) for the merchant to add for every `POST api/order/{merchantId}`

- Leverage 3rd party Logins (e.g. Google Login)<br />
Less "white box" than the above  where an _end user_ will have to authorize Gilded Rose application to his/her Google account profile. Merchants will `redirect` _end users_ to Gilded Rose web application to proceed with ordering.

#### Orders and Inventory

Inventory is always challenging, and several approaches are possible in the Api:

1. provide only available items in the inventory feed (implementation detail)
2. provide an actual count and let merchants decide how they want to handle
3. provide a "query" endoint for the merchant to use to check items availability "as needed" 
4. (dis)allow backorders 

Items `1` and `2` can be implemented in the `GET api/inventory/getInventory/{merchantId}` endpoint

Item `3` via `POST api/inventory/checkInventory/{merchantId}` for one or more items

Item `4` can be handled at `POST api/order/{merchantId}` , along with order validation, whether or not _backorders_ are "enabled" for a merchant (currently disallowed)


## Getting Started

This is a Visual Studio solution with 2 projects

1. **[Merchant Api](https://github.com/EdSF/GildedRose/tree/master/Merchant%20Api)**<br />
The Asp.Net MVC/Web Api Application
2. **[Merchant Api.Tests](https://github.com/EdSF/GildedRose/tree/master/Merchant%20Api.Tests)**<br />
Unit tests for the Merchant Api



### Prerequisities

1. .Net 4.6.1

2. Visual Studio 2015 ( _I'm not sure if `.Net 4.6.x` is available in Visual Studio 2013_ )
3. Optional: there is very simple implementation of Google Login which requires a Google Developer Account (for client ids, javascript origins, etc.). 
To enable/use:

    - Refer to [these instructions](https://developers.google.com/identity/sign-in/web/devconsole-project)
    - Obtain your Google Client ID from the Google developers console
    - **Important** - be sure the add the localhost:port used by Visual Studio as a Javascript origin - e.g. `http://localhost:55774`
    - create a file `ApiKeys.config` and add it to the **Merchant Api** Asp.Net MVC/API application root folder (same folder where `web.config` is located)
    - the contents of the file:


```
<appSettings>
  <!--Google APIs-->
  <add key="GoogleClientId" value="replace_this_with_your_Google_Client_ID"/>
</appSettings>
```


### Installing

After cloning the solution, Visual Studio 2015 should be able to obtain all the (Nuget) packages for the solution. As mentioned in the **Prerequisites** this includes [JWT .Net Library](https://www.nuget.org/packages/JWT/)

- Build the solution (both projets)


## Running the tests

Tests simulate both successful and failed HTTP request/responses including helpful debugging information for Api clients.

To run the tests:

- Run the Web MVC/Web API application locally (IIS Express) - <kbd>Ctrl</kbd>+<kbd>F5</kbd>
- In `Test Explorer`: Run all the Unit tests. Helpful outputs are available in the Output screen
- Alternatively, you can run "bare metal" http request/response tests using [`curl`](https://curl.haxx.se/download.html)

An example using `curl` that shows a failed request (line breaks only for clarity)
```
C:\>curl -v "http://localhost:55774/api/inventory/getinventory/MID001" 
         -H "Authorization: Token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJpQXBwcyIsImlhdCI6MTQyMTY4MTgwMSwiZXhwIjoxNDIxNjgyMTAxfQ._lNhUHwCHBdRRvt481e46hBIqxHMKYj56Mx0FF2AhLs"
```
Response
```
HTTP/1.1 401 Unauthorized
...ommitted for brevity...

"Invalid signature. Expected /lNhUHwCHBdRRvt481e46hBIqxHMKYj56Mx0FF2AhLs= got HrBsV/dFwEO/x+0SoFCNQYf4HvQ3oV/XWuMGQ9TsR08="
```

## Acknowledgments

* [JWT .Net Library Authors](https://www.nuget.org/packages/JWT/) John Sheehan, Michael Lehenbauer
