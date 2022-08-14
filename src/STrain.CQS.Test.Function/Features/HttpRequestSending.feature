Feature: [HRS] - HttpRequestSending

Sending request via HTTP to external services.

	 Rule: [HRS/SRS] - Sending request to STrain service
	
		If the target service uses STrain the communication can happen via generic endpoint. In this case the requests will be send with the following configuration:
			- __Command__:
				- Method: POST
				- Body: Serialized Command object in JSON format
			- __Query__:
				- Method: GET
				- Body: Serialized Query object in JSON format

		@issue-6
		@api	
		Scenario: [API][HRS/SRS-001] - Send command
			When Sending command to STrain service
			Then Request should be sent
				| BaseAddress            | Path | Method |
				| http://test-service/ | api  | POST   |

		@issue-6
		@api	
		Scenario: [API][HRS/SRS-002] - Send query
			When Sending query to STrain service
			Then Request should be sent
				| BaseAddress            | Path | Method |
				| http://test-service/ | api  | GET    |

	Rule: [HRS/SES] - Sending request to external service with attributive providers
	
		If the target sevice does not use STrain the communication will happen based on HttpRequestSender's configuration. For the configuration 5 components are needed:
			- __PathProvider__: Provides the path based on the request. It also can be used for route parameters.
			- __MethodProvider__: Provides the method based on the request.
			- __ParameterProvider__: Provides parameters based on the request. It has 3 types:
				- __HeaderParameterProvider__: Adds header parameters to the request.
				- __QueryParameterProvider__: Adds query parameters to the request.
				_ __BodyParameterProvider__: Add body parameter to the request.

		The base address comes from configuration.

		@issue-6
		@api
		Scenario Outline: [API][HRS/SES-001] - Send request
			When Sending '<Method>' request to external endpoint
			Then Request should be sent to external endpoint
				| BaseAddress          | Path   | Method   |
				| http://test-service/ | <Path> | <Method> |

			Examples: 
				| Description    | Method | Path              |
				| GET Request    | GET    | get-endpoint/1    |
				| POST Request   | POST   | post-endpoint/2   |
				| PUT Request    | PUT    | put-endpoint/3    |
				| PATCH Request  | PATCH  | patch-endpoint/4  |
				| DELETE Request | DELETE | delete-endpoint/5 |