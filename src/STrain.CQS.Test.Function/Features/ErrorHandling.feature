Feature: [ERH] - ErrorHandling

S-Train provides default error handling.

	Rule: [ERH/ERR] - Error response
		Used status codes:
		    - __400__: Validation or verification error.
			- __401__: Unathorized request.
			- __403__: Forbidden request.
			- __404__: Resource not found.
			- __500__: Internal servcer error.

	@issue-23
	@api
	Scenario: [API][ERH/ERR-001] - Resource not found
		//TODO Modify and use Calling..
		When Throwing NotFoundException
		Then Error response should be
			| Code | ContentType              | Type                       | Title               | Status | Detail                               | Instance |
			| 404  | application/problem+json | /errors/resource-not-found | Resource not found. | 404    | Resource '{resource}' was not found. | /api     |

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

	@issue-23
	@api
	Scenario: [API][ERH/ERR-004] - Forbidden request
		When Calling '/api/Sample/forbidden' endpoint
		Then Error response should be
			| Code | ContentType              | Type              | Title      | Status | Detail                                                                       | Instance              |
			| 403  | application/problem+json | /errors/forbidden | Forbidden. | 403    | Specific permission is required for access '/api/Sample/forbidden' endpoint. | /api/Sample/forbidden |

	@issue-23
	@api
	Scenario: [API][ERH/ERR-005] - Internal server error
		When Calling '/api/Sample/internal-server-error' endpoint
		Then Error response should be
			| Code | ContentType              | Type                          | Title                  | Status | Detail                                               | Instance                          |
			| 500  | application/problem+json | /errors/internal-server-error | Internal server error. | 500    | Unexpected error happened. Please, call the support. | /api/Sample/internal-server-error |
				
	@issue-23
	@api
	Scenario: [API][ERH/ERR-006] - Validation error
		When Calling '/api/Sample/invalid-request' endpoint
		Then Error response should be
			| Code | ContentType              | Type                  | Title            | Status | Detail                               | Instance                    | Errors.Property | Errors.Message        |
			| 400  | application/problem+json | /errors/invalid-value | Invalid Request. | 400    | Invalid request. See the the errors. | /api/Sample/invalid-request | Value           | Value cannot be empty. |
				
	@issue-23
	@api
	Scenario: [API][ERH/ERR-007] - Verification error
		When Calling '/api/Sample/verification-error' endpoint
		Then Error response should be
			| Code | ContentType              | Type                              | Title               | Status | Detail                                                                    | Instance                       |
			| 400  | application/problem+json | /errors/custom-verification-error | Verification error. | 400    | Custom verification error. Can be used for business logic related errors. | /api/Sample/verification-error |