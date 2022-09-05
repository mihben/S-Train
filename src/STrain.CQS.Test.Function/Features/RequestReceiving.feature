Feature: [RQR] - RequestReceiving

S-Train is capable to receiving requests. Supported requests:
	- __Command__
	- __Query__

	Rule: [RQR/MVC] - Receiving requests via MVC
		
		The MVC request receiver responses:
			- __Command__:
				- Content: empty
				- StatusCode: 202 - Accepted
			- __Query__:
				- Content: Result of the query
				- StatusCode: 200 - Ok

		@api
		@issue-6
		Scenario: [API][RQR/MVC-001] - Receiving command
			Given Registered command performer
			When Receiving command
			Then Performer should be called

		@api
		@issue-6
		Scenario: [API][RQR/MVC-002] - Receiving query
			Given Registered query performer
			When Receiving query
			Then Performer should be called

	Rule: [RQR/AUT] - Authorize requests
		
		Request authorization happens based on [Authorize] attribute.

		@issue-24
		@api
		Scenario: [API][RQR/AUT-001] - Request authorization
			When Receiving authorized request
			Then Authorization response should be
				| StatusCode |
				| 202        |

		@issue-24
		@api
		Scenario: [API][RQR/AUT-002] - Unathorized request
			When Receiving unauthorized request
			Then Authorization response should be
				| StatusCode |
				| 401        |

		@issue-24
		@api
		Scenario: [API][RQR/AUT-003] - Forbidden request
			When Receiving forbidden request
			Then Authorization response should be
				| StatusCode |
				| 403        |

		@issue-24
		@api
		Scenario: [API][RQR/AUT-004] - Allow anonymus
			When Receiving allow anonymus request
			Then Authorization response should be
				| StatusCode |
				| 202        |