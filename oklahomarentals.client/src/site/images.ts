import { Cloudinary } from '@cloudinary/url-gen'
import { scale } from '@cloudinary/url-gen/actions/resize'

const cld = new Cloudinary({ cloud: { cloudName: 'djt05lgzv' } })

// URL for a named image in the Cloudinary media library, scaled down to `width`.
export function cloudinaryUrl(publicId: string, width = 1600): string {
    return cld.image(publicId).format('auto').quality('auto').resize(scale().width(width)).toURL()
}

// Adds format/quality/width transformations to a full Cloudinary delivery URL
// (the property photos the API returns are untransformed originals).
export function sizedUrl(url: string, width: number): string {
    return url.includes('/upload/') ? url.replace('/upload/', `/upload/f_auto,q_auto,c_limit,w_${width}/`) : url
}
