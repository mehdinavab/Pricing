# Development SuperAdmin seed

Run the API once with `--seed-superadmin` and ASPNETCORE_ENVIRONMENT=Development. Supply IdentitySeed__Username and IdentitySeed__Password through environment variables (or equivalent user-secret keys). Do not commit the password. ConnectionStrings__IdentityDb selects the target; the host otherwise uses its normal configured Identity connection.

The command applies only Identity migrations, seeds existing roles and creates the explicitly requested SuperAdmin, then exits without opening an HTTP listener. It uses ASP.NET Core Identity hashing and user/role stores in a transaction. Repeating it verifies the existing password and role without duplicating records or replacing an existing password. Normal API startup never runs this seed.

The development bootstrap accepts the supplied initial password independently of normal password complexity. This exception is restricted to Development. Normal user creation/reset policies remain unchanged. Store a production account password through the existing authorized user-management flow.
