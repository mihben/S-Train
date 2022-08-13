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
		@request-sender
		Scenario: [API][HRS/SRS-001] - Send command
			When Sending command to STrain service
			Then Request should be sent
				| BaseAddress            | Path | Method |
				| http://strain-service/ | api  | POST   |

		@issue-6
		@api	
		@request-sender
		Scenario: [API][HRS/SRS-002] - Send query
			When Sending query to STrain service
			Then Request should be sent
				| Url                    | Path | Method |
				| http://strain-service/ | api  | GET    |