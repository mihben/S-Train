Feature: [RQS] - RequestSending

Sending request to external service. Currently only HTTP is supported.

	 Rule: [RQS/SNR] - Sending requests
	
		If the target service uses STrain, the communication can happen via generic endpoint. In this case the requests will be send with the following configuration:
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
		Scenario: [API][RQS/SNR-001] - Send generic command
			Given Configured HTTP sender
				| Key     | BaseAddress             | Path        |
				| Generic | http://generic-service/ | generic-api |
			When Sending generic command
			Then Request should be sent to
				| BaseAddress             | Path        | Method |
				| http://generic-service/ | generic-api | POST   |

		@issue-17
		@api	
		Scenario: [API][RQS/SNR-002] - Send generic query 
			Given Configured HTTP sender
				| Key     | BaseAddress             | Path        |
				| Generic | http://generic-service/ | generic-api |
			When Sending generic query
			Then Request should be sent to
				| BaseAddress             | Path        | Method |
				| http://generic-service/ | generic-api | GET    |

#		@issue-17
#		@api
#		Scenario Outline: [API][RQS/SNR-003] - Send request to external service
#			Given Configured HTTP sender
#				| Key      | BaseAddress              |
#				| External | http://external-service/ |
#			When Sending external '<Method>' request
#			Then Request should be sent to
#				| BaseAddress              | Path   | Method   |
#				| http://external-service/ | <Path> | <Method> |
#
#			Examples: 
#				| Description    | Method | Path                         |
#				| GET Request    | GET    | external-api/get-endpoint    |
#				| POST Request   | POST   | external-api/post-endpoint   |
#				| PUT Request    | PUT    | external-api/put-endpoint    |
#				| PATCH Request  | PATCH  | external-api/patch-endpoint  |
#				| DELETE Request | DELETE | external-api/delete-endpoint |

	Rule: [RQS/ERH] - Generic error response handling

		Error response handler can be injected to the http request sender.

		@issue-23
		@api
		Scenario: [API][RQS/ERH-001] - Not found
			When Generic request is responding
				| ContentType              | Type                       | Title               | Status | Detail                               | Instance |
				| application/problem+json | /errors/resource-not-found | Resource not found. | 404    | Resource '{resource}' was not found. | /api     |
			Then Should be thrown 'NotFoundException'

		@issue-23
		@api
		Scenario Outline: [API][RQS/ERH-002] - Internal server error
			When Generic request is responding
				| ContentType              | Type   | Title   | Status   | Detail   | Instance   |
				| application/problem+json | <Type> | <Title> | <Status> | <Detail> | <Instance> |
			Then Should be thrown 'HttpRequestException'
		
		Examples: 
			| Description           | Type                          | Title                  | Status | Detail                                                                            | Instance                                   |
			| Unathorized           | /errors/unathorized           | Unathorized request.   | 401    | Authentication is required for access '/api/Sample/authorized-endpoint' endpoint. | /api/Sample/authorized-endpoint            |
			| Forbidden             | /errors/forbidden             | Forbidden.             | 403    | Specific permission is required for access '/api/Sample/forbidden' endpoint.      | /api/Sample/forbidden-endpoint             |
			| Internal Server Error | /errors/internal-server-error | Internal server error. | 500    | Unexpected error happened. Please, call the support.                              | /api/Sample/internal-server-error-endpoint |

		@issue-23
		@api
		Scenario: [API][RQS/ERH-003] - Validation error
			When Generic request is responding
				| ContentType              | Type                    | Title            | Status | Detail                           | Instance                    | Errors.Property | Errors.Message                 |
				| application/problem+json | /errors/invalid-request | Invalid request. | 400    | Invalid request. See the errors. | /api/Sample/invalid-request | Parameter       | 'Parameter' must not be empty. |
			Then Should be thrown 'ValidationException'

		@issue-23
		@api
		Scenario: [API][RQS/ERH-004] - Verification error
			When Generic request is responding
				| ContentType              | Type                              | Title                      | Status | Detail                                        | Instance                              |
				| application/problem+json | /errors/custom-verification-error | Custom verification error. | 400    | This is a custom verification error for test. | /api/Sample/custom-verification-error |
			Then Should be thrown 'VerificationException'