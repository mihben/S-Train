Feature: [GRH] - GenericRequestHandling

Possible to recieve request via generic endpoint. The request type is determined based on the ___request-type___ header in ___'{namespace}.{requestType}, {assembly}'___ format. Assembly must not contain version information.

	Rule: [GRH/HC] - Handle Command
		
		Handle commands by generic request handler. Configurations:
			- __Path__: configurable (default: /api)
			- __Method__: POST
			- __StatusCode__: 202 - Accepted
			
		@issue-14
		@api
		Scenario: [API][GRH/HC-001] - Receive Command
			When Receiving command via generic controller
			Then Response should be
				| StatusCode | Content |
				| 202        |         |

	Rule: [GRH/HQ] - Handle Query
	
		Handle queries by generic request handler. Configuration:
			- __Path__: configurable (default: /api)
			- __Method__: GET
			- __StatusCode__: 200 - Ok

		@issue-14
		@api
		Scenario: [API][[GRH/HQ-001] - Receive Query
			When Receiving query via generic controller
			Then Response should be
				| StatusCode | Content              |
				| 200        | Handled by performer |