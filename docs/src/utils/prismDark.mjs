/**
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
import {themes} from 'prism-react-renderer';
const darkTheme = themes.vsDark;

export default {
  plain: {
    color: '#D4D4D4',
    backgroundColor: '#272727',
  },
  styles: [
    ...darkTheme.styles,
  ],
};