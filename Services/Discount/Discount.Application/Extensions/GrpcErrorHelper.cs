using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Google.Rpc;
using Grpc.Core;
//! and here
using GoogleStatus = Google.Rpc.Status;
using GrpcStatus = Grpc.Core.Status;

namespace Discount.Application.Extensions
{
	public static class GrpcErrorHelper
	{
		public static RpcException CreateValidationException(Dictionary<string, string> fieldErrors)
		{
			var fieldViolations = new List<BadRequest.Types.FieldViolation>();
			foreach (var error in fieldErrors)
			{
				fieldViolations.Add(
					new BadRequest.Types.FieldViolation { Field = error.Key, Description = error.Value }
				);
			}

			// add bad request
			var badRequest = new BadRequest();
			badRequest.FieldViolations.AddRange(fieldViolations);

			//! here
			var status = new GoogleStatus
			{
				Code = (int)StatusCode.InvalidArgument,
				Message = "Validation failed",
				Details = { Any.Pack(badRequest) },
			};

			var trailers = new Metadata { { "grpc-status-details-bin", status.ToByteArray() } };

			//! idk why this is not working (the fix is at the top and below this piece of code)
			// return new RpcException(
			// 	new Grpc.Core.Status(StatusCode.InvalidArgument, "Validation errors"),
			// 	trailers
			// );

			return new RpcException(
				new GrpcStatus(StatusCode.InvalidArgument, "Validation errors"),
				trailers
			);
		}
	}
}
