# Image Processor Project

## Overview
The Image Processor project is a serverless application deployed on AWS, consisting of a .NET 8 backend running on AWS Lambda and an Angular frontend hosted on S3.

## Project Structure
- **Frontend**: Angular UI hosted on AWS S3 and accessible via CloudFront.
- **Backend**: .NET 8 Web API deployed as an AWS Lambda function, exposed via API Gateway.
- **Event Processing**: Uses Amazon Kinesis for event-driven architecture.

## URLs
- **Frontend:** [Image Processor UI](http://amzn-s3-frontend.s3-website.eu-west-3.amazonaws.com/)
- **API Swagger UI (opened in prod for exam purpose):** [AWS Lambda API Documentation](https://7nz6jxmvf4.execute-api.eu-west-3.amazonaws.com/Prod/swagger/index.html)

## Setup & Deployment
### Backend (.NET 8 Lambda API)
1. **Build & Publish**
   ```sh
   dotnet publish -c Release
   ```
2. **Deploy to AWS Lambda**
   ```sh
   aws lambda update-function-code --function-name ImageProcessorLambda --zip-file fileb://path-to-zip.zip
   ```
3. **Update API Gateway if needed**
   ```sh
   aws apigateway create-deployment --rest-api-id your-api-id --stage-name Prod
   ```

### Frontend (Angular S3 Hosting)
1. **Build Angular App**
   ```sh
   ng build --configuration=production
   ```
2. **Sync with S3**
   ```sh
   aws s3 sync dist/image-ui/ s3://your-bucket-name --acl public-read
   ```
3. **Invalidate CloudFront Cache (if applicable)**
   ```sh
   aws cloudfront create-invalidation --distribution-id YOUR_DISTRIBUTION_ID --paths "/*"
   ```

### Kinesis Stream Testing
1. **Send Event to Kinesis**
   ```sh
   aws kinesis put-record --stream-name kinesis-ds-aleo --partition-key testKey --data "$(echo -n '{\"ImageUrl\":\"http://example.com/image.jpg\", \"Description\": \"Test image event from Kinesis\"}' | base64)"
   ```
2. **Check AWS CloudWatch Logs** for Lambda execution.

## Challenges & Enhancements
### Challenges
- **Shared Cache Issue**: Could not maintain a shared in-memory list or cache for `KinesisEventConsumer`.
- **Tightly Coupled API Calls**: Kinesis is making direct API calls due to lack of shared state.

### Future Enhancements
- **Implement Redis**: Use AWS ElastiCache to store shared state.
- **DynamoDB Integration**: Replace direct API calls with event-driven DynamoDB persistence.
- **Refactor for Clean Architecture**: Improve separation of concerns in the backend.

---
### Contributors
Maintained by [Amine Abdalkhalki](https://github.com/amineabdalkhalki)

