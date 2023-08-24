
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI_Coupon.Data;
using MinimalAPI_Coupon.Models;
using MinimalAPI_Coupon.Models.DTOs;
using MinimalAPI_Coupon.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPI_Coupon {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();    

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddValidatorsFromAssemblyContaining<CouponCreateDTO>();
            //builder.Services.AddAutoMapper(typeof(Program).Assembly);
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            //Get all coupons
            app.MapGet("/api/coupons", () => {
                APIResponse response = new APIResponse();

                response.Result = CouponStore.couponList;
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;

                return Results.Ok(response);
            }).WithName("GetAllCoupons")
            .Produces<IEnumerable<CouponDTO>>(200);

            app.MapGet("/api/coupon/{id:int}", (int id) => {
                APIResponse response = new APIResponse();

                response.Result = CouponStore.couponList.FirstOrDefault(x => x.Id == id);
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;

                return Results.Ok(response);
            }).WithName("GetCouponById");

            ////Create coupon
            //app.MapPost("/api/coupon", ([FromBody] Coupon coupon) => {
            //    if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name)) {
            //        return Results.BadRequest("Invalid ID or Coupon Name");
            //    }
            //    if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon.Name.ToLower()) != null) {
            //        return Results.BadRequest("Coupon Name already exists.");
            //    }

            //    coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
            //    CouponStore.couponList.Add(coupon);
            //    return Results.Created($"/api/coupon/{coupon.Id}", coupon);
            //}).Produces<Coupon>(201).Produces(400);

            //Create coupon
            app.MapPost("/api/coupon", async (IValidator<CouponCreateDTO> validator, IMapper _mapper, [FromBody] CouponCreateDTO coupon_c_dto) => {

                APIResponse response = new () { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
                
                var validationResult = await validator.ValidateAsync(coupon_c_dto);
                if (!validationResult.IsValid) {
                    return Results.BadRequest(response);
                }
                //Check if the coupon already exists by Id and Name
                if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon_c_dto.Name.ToLower()) != null) {
                    response.ErrorMessages.Add("Coupon Name already exists");
                    return Results.Ok(response);
                }

                //Instead doing this manually we can use automapper
                //Coupon coupon = new Coupon {
                //    IsActive = coupon_c_dto.IsActive,
                //    Name = coupon_c_dto.Name,
                //    Percent = coupon_c_dto.Percent
                //};

                //Automapper
                Coupon coupon = _mapper.Map<Coupon>(coupon_c_dto);

                coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                CouponStore.couponList.Add(coupon);

                CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

                response.Result = couponDTO;
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.Created;

                return Results.Ok(response);
            }).WithName("CreateCoupon")
            .Accepts<CouponCreateDTO>("application/json")
            .Produces<APIResponse>(201).Produces(400);

            //Update coupon
            app.MapPut("/api/coupon", async (IValidator <CouponUpdateDTO> _validator, IMapper _mapper, [FromBody]CouponUpdateDTO coupon_u_dto) => {

                APIResponse response = new () { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest};

                var validResult = await _validator.ValidateAsync(coupon_u_dto);
                if (!validResult.IsValid) {
                    response.ErrorMessages.Add(validResult.Errors.FirstOrDefault().ToString());
                }

                Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.Id == coupon_u_dto.Id);
                couponFromStore.IsActive = coupon_u_dto.IsActive;
                couponFromStore.Name = coupon_u_dto.Name;
                couponFromStore.Percent = coupon_u_dto.Percent;
                couponFromStore.LastUpdate = DateTime.Now;

                Coupon coupon = _mapper.Map<Coupon>(couponFromStore);
                response.Result = _mapper.Map<CouponDTO>(couponFromStore);
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;

                return Results.Ok(response);

            }).WithName("UpdateCoupon")
            .Accepts<CouponUpdateDTO>("application/json")
            .Produces<APIResponse>(200);


            //Delete coupon
            app.MapDelete("/api/coupon/{id:int}", (int id) => {
                APIResponse response = new APIResponse() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };

                //Check if the coupon exist by Id
                Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
                if (couponFromStore != null) {
                    CouponStore.couponList.Remove(couponFromStore);
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;

                    return Results.Ok(response);
                }
                
                response.ErrorMessages.Add("Invalid Id");
                return Results.BadRequest(response);
            });

            app.Run();
        }
    }
}