# Axlon Files service

`Axlon.Services.Files` currently uses the private `local` provider for pre-production testing. Final objects are stored under `wwwroot/files/tenants/...`; multipart staging data is stored under `wwwroot/.uploads/...`. The application intentionally does not enable `UseStaticFiles`, so knowing the disk path does not grant download access.

## HTTP interface

The public interface contains only upload, download, and preview. Local storage and OSS use separate controllers so callers never pass a provider name or participate in signing and multipart state.

| Storage | Upload | Download | Preview |
| --- | --- | --- | --- |
| Local | `POST /api/files/local/upload` | `GET /api/files/local/{id}/download` | `GET /api/files/local/{id}/preview` |
| OSS | `POST /api/files/oss/upload` | `GET /api/files/oss/{id}/download` | `GET /api/files/oss/{id}/preview` |

Upload uses `multipart/form-data` with a real `file` field and a `visibility` field (`private` or `tenant`). Download and preview authorize the caller and redirect to a 15-minute protected URL. Signing, part upload, completion, ETag collection, size checks, MIME checks, and magic-byte checks are implementation details hidden behind these routes.

Local signed URLs are encrypted bearer credentials. Do not log, persist, or share them. Upload and merge operations stream data and never buffer a complete image or video in memory.

Signed URLs use the current external request origin after forwarded-header processing. Set `Axlon:Files:Providers:local:PublicBaseUrl` only when the Files service cannot infer the externally reachable origin from the request.

## Storage locations

- Project run: `services/Axlon.Services.Files/wwwroot/files`
- Docker final objects: `/app/wwwroot/files`
- Docker multipart staging: `/app/wwwroot/.uploads`
- Compose persists these directories under the repository `storage/` folder, which is ignored by Git and Docker build context.

## Switching back to OSS

Set `Axlon:Files:Providers:oss:Enabled=true` and provide `OSS_ACCESS_KEY_ID`, `OSS_ACCESS_KEY_SECRET`, `OSS_REGION`, `OSS_ENDPOINT`, and `OSS_BUCKET`. The OSS controller then becomes available at `/api/files/oss`; the local controller remains independent. Existing records keep their stored provider, bucket, and object key.

For mainland China buckets that require CNAME data access, set `OSS_ENDPOINT` to the HTTPS custom domain and `OSS_USE_CNAME=true`. Keep the bucket private, configure exact CORS origins, allow `PUT`, `GET`, and `HEAD`, and expose `ETag`.
