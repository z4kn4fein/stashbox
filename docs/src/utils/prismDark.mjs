/**
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

import darkTheme from 'prism-react-renderer/themes/vsDark/index.cjs.js';

export default {
  plain: {
    color: '#D4D4D4',
    backgroundColor: '#272727',
  },
  styles: [
    ...darkTheme.styles,
  ],
};