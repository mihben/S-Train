Feature: [ERH] - ErrorHanling

S-Train provides default error handling.

	Rule: [ERH/ERR] - Error response

	@issue-23
	@api
	Scenario: [API][ERH/ERR-001] - Resource not found
		When Throwing NotFoundException
		Then Error response should be
			| Code | ContentType              | Type                       | Title               | Status | Detail                               | Instance                   |
			| 404  | application/problem+json | /errors/resource-not-found | Resource not found. | 404    | Resource '{resource}' was not found. | /api;SampleNotFoundCommand |

	@issue-23
	@api
	Scenario: [API][ERH/ERR-002] - Endpoint not found
		When Calling '/api/fake-endpoint' endpoint
		Then Error response should be
			| Code | ContentType              | Type                       | Title               | Status | Detail                                       | Instance           |
			| 404  | application/problem+json | /errors/endpoint-not-found | Endpoint not found. | 404    | Endpoint '/api/fake-endpoint' was not found. | /api/fake-endpoint |

	@issue-23
	@api
	Scenario: [API][ERH/ERR-003] - Unathorized request
		When Calling '/api/Sample/authorized-endpoint' endpoint
		Then Error response should be
			| Code | ContentType              | Type                | Title                | Status | Detail                                                                            | Instance                        |
			| 401  | application/problem+json | /errors/unathorized | Unathorized request. | 401    | Authentication is required for access '/api/Sample/authorized-endpoint' endpoint. | /api/Sample/authorized-endpoint |