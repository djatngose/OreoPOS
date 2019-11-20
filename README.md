# Business Logic

* This document to help understand the business logic in running code and also up-to-date with frequent changes from client
* **Payment Gateway**: Currently working only `Stripe` payment gateway and Currency is `USD`

## I. Stripe Payment Gateway

### 1. Overview
  * #### Products:
      > A product is something to which your customers subscribe. 
    
      `For example: ` the enterprise tier of a software product or usage of an API. You'll add plans next to define this product's pricing.    

    **Payload**:
    ``` javascript
    {
      "id": "prod_GCtGtlUxryb2xz",
      "object": "product",
      "active": true,
      "attributes": [],
      "caption": null,
      "created": 1574157500,
      "deactivate_on": [],
      "description": null,
      "name": "Appvert",
      "type": "service",
      "unit_label": null,
      "updated": 1574157500,
      //...
    }
    ```
    **Atributes**:

      * `name`: This will appear on Checkout, customers' receipts, and invoices.

      * `unit_label`: This will represent a unit of this product, such as a seat or API call, on Checkout, customers' receipts, and invoices.

  * #### Plans
      > Plans define how much and how frequently you charge for a product. This includes the base price, currency, and billing cycle for subscriptions. 
    
      `For example: ` you might have a $5/month plan that provides limited access to your products, and a $15/month plan that allows full access.

    **Payload**:
    ``` javascript
    {
      "id": "plan_GCtG6dASG8r2H5",
      "object": "plan A",
      "active": true,
      "amount": 200000,
      "amount_decimal": "200000",
      "billing_scheme": "per_unit",
      "created": 1574157500,
      "currency": "usd",
      "interval": "month",
      "interval_count": 1,
      "livemode": false,
      "product": "prod_GCtGtlUxryb2xz",
      //...
    }   
    ```
    **Atributes**:

      * `interval: One of `day, week, month or year`. The frequency with which a subscription should be billed.

      * `product`: The product whose pricing this plan determines.

  * #### Subscription
      > Subscriptions allow you to charge a customer on a recurring basis.

    **Payload**:
    ``` javascript
    {
      "id": "sub_GCsWpJzfKIHxKq",
      "object": "subscription",  
      "application_fee_percent": null,
      "billing_cycle_anchor": 1574154757,
      "current_period_end": 1576746757,
      "current_period_start": 1574154757,
      "customer": "cus_G2OLrSXLhcSs91",
        "plan": {
          "id": "plan_G8Hw6X927a4xYa",
          "object": "plan",
          "active": true,   
          "amount": 5000,
          "amount_decimal": "5000",
          "billing_scheme": "per_unit",
          "created": 1573096157,
          "currency": "usd",
          "interval": "month",
          "interval_count": 1,
          "product": "prod_G8HwKPxUhVA9iP",
          },
          //...
    }
    ```

### 2. Set up a subscription

### 3. Create an invoice

## II. Subscription flows

> There are main steps that describe subscription flows of AppVert

### 1. Admin create a `Plan`

* **Name**: `unique name` (make sure check duplicate name before saving)
* There are `3` recurring types of plan to choose: `Daily, Weekly, Monthly`
* Have 2 types of Plan :
  * **Free**: only create a `Plan` to `AppVert DB`
  * **Paid**:
    * Create a `Plan` to `AppVert DB`
    * Create a [Product](#Products) and also a [Plan](#Plans) to `Stripe DB`
    * Update `StripePlanReferenceId` column of the Plan to `AppVert DB`
* **Cost**: is available when choose `Paid` type
* **Pseudocode**:

  ``` csharp
  public void AddPlanAsync(PlanDto dto)
  {
    PlanDto dto;
    if (CheckDuplicateName(dto.Name))
    {
      // ...
      throw error;
    }
    if (dto.Type == 'Paid')
    {
      // Call Stripe API to create a plan
      var stripePlan = await stripeAPI.CreatePlan(new StripePlanDto
        {
          Product = new StripeProduct()            
            Name = model.Product.Name
          },
          Amount = (int)(dto.Cost * 100),
          Currency = StripeConstants.PLAN_CURRENCY, // 'USD'
          // ...
      });

      dto.StripePlanReferenceId = stripePlan.Id;
    }
    var newPlan = new Plan
    {
      Name = dto.Name,
      ImpressionQuota = dto.ImpressionQuota,
      StripePlanReferenceId = dto.StripePlanReferenceId,
      // ...
    };
    _appVertContext.SaveToDB(newPlan);
  }
  ```

* **Issues**:

  > There is only a problem when create a plan, `Paypal`'s still working and also create a plan too.

### 2. Admin update a `Plan`

* **Name**: `unique name` (make sure check duplicate name before saving)
* **Check** the different of `ImpressionQuota, Cost, Name and Plan Type`
  * **Yes**:
    * `Inactive` the current plan
    * Add a new plan for both `AppVert and Stripe` - like [Step 1](#1.-Admin-create-a-plan)
  * **No**:
    * `Only update` the current plan as `status, description, modified by, modified date`
* **Pseudocode**:

    ``` csharp
    public async Task UpdatePlanAsync(PlanDto planDto, string userName)
    {
      var currentPlan = await Repository.GetPlan(planDto.Id);

      if (planDto.Status == 'Active')
      {
        if (CheckDuplicateName(planDto.Name))
        {
          // ...
          throw error;
        }

        if (
          planDto.ImpressionQuota != currentPlan.ImpressionQuota ||
          planDto.Cost != currentPlan.Cost ||
          planDto.Name != currentPlan.Name ||
          planDto.Type != currentPlan.Type
        )
        {
          currentPlan.Status = 'Inactive';
          Repository.UpdateDB(currentPlan);

          AddNewPlan(planDtoe);
        }
        else
        {
          currentPlan.Description = dto.Description;
          currentPlan.Status = dto.Status;
          // ...
          Repository.UpdateDB(orginal);
        }
    }
    ```

### 3. Admin disable a plan

  > Only update the current plan's properties as `inactive status, description, modified by, modified date`

### 2. Register a paid plan

  * After register a new user succesfully, the system will insert a `default Plan (Free Plan)`

  * Afterward, if they subscribe `PayPal/Stripe` successfully, the system will update to `Paid Plan`

  * **Pseudocode**:

    ``` csharp
    public void RegisterAccount(User userDto)
    {
      // validation
      // user is created successfully

      var freePlan = Get_Free_Plan();

      var subDto = new SubscriptionDto
      {
        PlanId = freePlan.Id,
        TempPlanId = userDto.SelectedPlanId == 'Free Plan' ? null : 'Paid Plan',
        StartDate = DateTime.Now,
        NextPaymentDate = CaculateDateNowTo(DateTime.Now, 'Wekkly' | 'Daily' | 'Monthly') 
        RemainingQuota = plan.ImpressionQuota,
        TotalQuota = plan.ImpressionQuota,                  
      };

      Create_Subscription_With_FreePlan(subDto);
      
      Create_Subscription_To_Stripe_DB();
    }
    ```

### 3. Using a plan

### 4. Upgrade a plan

* If payment method `(Stripe/Paypal)` of user is not exist:
  * Create `Stripe's Customer`

  * Create `Appvert's payment method`

* `Create new` Stripe's Subscription
* `Cancel current` Stripe's Subscription if exist
* If current subscription's `plan` and  `status`:
  * plan = `Free` | status = `Failed` | status = `Subspend` : Update `new Remaining Quota and Total Quota`

  * `Otherwise` : `Accumulate` the  `Remaining Quota and Total Quota`
* Create `new` Subscription of `AppVert`

### 5. Downgrade a plan

* If payment method `(Stripe/Paypal)` of user is not exist:
  * Create `Stripe's Customer`

  * Create `Appvert's payment method`

* `Create new` Stripe's Subscription
* `Cancel current` Stripe's Subscription if exist
* Create `Waiting` Subscription for `next recurring`
  * Update `new Remaining Quota and TotalQuota` with impression quota of the `Plan`
* Update `current` Subscription
  * No `TempPlan Id`
  * Status : `active`

### 6. Subspend a plan

### 7. Resume a plan

### 8. Cancel a plan

### 9. Plan Recurring

* Reset Quota of  Upcoming Subscriptions Of Free Plans every day. For purpose, make sure get the most update quota of free plans

* Find the cancel subscriptions are waiting.Set the current subscription to Cancel and switch the account to Free Plan. Reset uesd budget

### 10. Stripe/Paypal webhook

* Any event manipulation occurs on `Stripe/ Paypal` function that will `notify` to `AppVert` system
* [Stripe web hook link](https://appvert.azurewebsites.net/api/stripewebhook)

### 11. Issues

  1. When subscribe a plan, if admin updates the plan to inactive status, how 's subscription of the plan ? Is it also inactive too?
  2. There are no signal to inform Stripe that the subscription is not avaiable then Stripe also cancel them?
  3. When create a plan, Paypal also create their own plan too? But the requirement already removed Paypal feature
  4. If downgrade the subscription, so the impression quota of downgrade plan < current plan, what happen about budget campaign > impression quota of downgrade plan

## Payment History

  Currently , It gets from 3 tables (```Payment, Invoice, Subscription and Plan```) from local DB

## Frequently asked questions (FAQs)

  > [Subscription questions](https://groovetech.atlassian.net/wiki/spaces/PA/pages/744259640/Subscription+questions)

* **Question 1** - Do we use Subscription function of Stripe OR code by ourselves for AppVert's Subscription?

  * Currently, we are using Stripe APIs for create a product ,plan, subscription and invoice, also have webhook to detect what changes on stripe and notify to AppVert
  * Updgrade, downgrade
    * Cancel current stripe subscription
    * Create new Stripe subscription

* **Question 2** - Subscription flow diagram

  * Upgrade plan process
  * Downgrade plan process
