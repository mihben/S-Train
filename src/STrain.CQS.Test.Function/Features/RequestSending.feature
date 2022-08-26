Feature: [RQS] - RequestSending

Sending request to external service. Currently only HTTP is supported.

	 Rule: [RQS/SRH] - Sending requests via HTTP
	
		If the target service uses STrain the communication can happen via generic endpoint. In this case the requests will be send with the following configuration:
			- __Command__:
				- Method: POST
				- Body: Serialized Command object in JSON format
			- __Query__:
				- Method: GET
				- Body: Serialized Query object in JSON format
		
		Otherwise the request will be sent according to attribute configurations. For the configuration 5 components are needed:
			- __PathProvider__: Provides the path based on the request. It also can be used for route parameters.
			- __MethodProvider__: Provides the method based on the request.
			- __ParameterProvider__: Provides parameters based on the request. It has 3 types:
				- __HeaderParameterProvider__: Adds header parameters to the request.
				- __QueryParameterProvider__: Adds query parameters to the request.
				_ __BodyParameterProvider__: Add body parameter to the request.
				
		The base address comes from configuration.

		@issue-17
		@api	
		Scenario: [API][RQS/SRH-001] - Send generic command
			When Sending command to STrain service
			Then Request should be sent
				| BaseAddress            | Path | Method |
				| http://strain-service/ | api  | POST   |

		@issue-17
		@api	
		Scenario: [API][RQS/SRH-002] - Send generic query 
			When Sending query to STrain service
			Then Request should be sent
				| BaseAddress          | Path | Method |
				| http://strain-service/ | api  | GET    |

		@issue-17
		@api
		Scenario Outline: [API][RQS/SRH-003] - Send request to external service
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

	Rule: [RQS/RRT] - Request routing

		Can be configured multiple request sender. In this case the request router is determine the right request sender according to the request.

		@blocked
		@issue-20
		@api
		Scenario: [API][RQS/RRT-001] - Send generic request
			Given Configured generic request sender to 'http://strain-service/'
			And Configured external request sender to 'http://test-service/'
			When Sending generic request
			Then Request should be sent to 'http://strain-service/'

		@blocked
		@issue-20
		@api
		Scenario: [API][RQS/RRT-002] - Send external request
			Given Configured generic request sender to 'http://strain-service/'
			And Configured external request sender to 'http://test-service/'
			When Sending external request
			Then Request should be sent to 'http://test-service/'